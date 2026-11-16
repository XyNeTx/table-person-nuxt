<template>
  <title>Modal View Nuxt</title>
  <div class="flex flex-col items-center justify-center min-h-screen py-2 bg-gray-100">
    <div class="w-fit min-w-4xl bg-white p-6 rounded shadow-md">
      <div class="flex justify-end mb-1">
        <Button
          @click="addPerson()"
          class="bg-green-500 hover:bg-green-700 text-white font-bold py-1 px-2 rounded"
        >
          ADD
        </Button>
      </div>
      <table class="table-auto border-collapse border border-slate-500 min-w-4xl">
        <thead class="bg-cyan-500">
          <tr>
            <th class="border border-slate-600 px-4 py-2">ID</th>
            <th class="border border-slate-600 px-4 py-2">ชื่อ-สกุล</th>
            <th class="border border-slate-600 px-4 py-2">ที่อยู่</th>
            <th class="border border-slate-600 px-4 py-2">วันเกิด</th>
            <th class="border border-slate-600 px-4 py-2">อายุ</th>
            <th class="border border-slate-600 px-4 py-2">Action</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="person in persons" :key="person.id">
            <td class="border border-slate-700 px-4 py-2">{{ person.id }}</td>
            <td class="border border-slate-700 px-4 py-2">{{ person.name }} {{ person.surname }}</td>
            <td class="border border-slate-700 px-4 py-2">{{ person.address }}</td>
            <td class="border border-slate-700 px-4 py-2 text-center">{{ person.dateOfBirth }}</td>
            <td class="border border-slate-700 px-4 py-2 text-center">{{ person.age }}</td>
            <td class="border border-slate-700 px-4 py-2 text-center">
              <Button
                @click="openModal(person)"
                class="bg-blue-500 hover:bg-blue-700 text-white font-bold py-1 px-2 rounded"
              >
                View
              </Button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
  <Dialog v-model:open="isModalOpen">
    <DialogContent class="max-h-[90vh] w-[calc(100vw-2rem)] max-w-none overflow-x-hidden overflow-y-auto bg-white p-6 sm:w-[min(90vw,48rem)] sm:p-8">
      <DialogHeader>
        <DialogTitle>{{ isReadOnly ? 'ข้อมูลบุคคล' : isEditing ? 'แก้ไขข้อมูลบุคคล' : 'เพิ่มข้อมูลบุคคล' }}</DialogTitle>
        <DialogDescription>{{ isReadOnly ? 'ดูรายละเอียดข้อมูลบุคคล' : 'กรอกข้อมูลบุคคลให้ครบถ้วน' }}</DialogDescription>
      </DialogHeader>
      <form class="space-y-5" @submit.prevent="savePerson">
        <div class="grid min-w-0 grid-cols-[6rem_minmax(0,1fr)_minmax(0,1fr)] items-center gap-3">
          <label for="person-name" class="font-medium">ชื่อ - สกุล</label>
          <input
            id="person-name"
            v-model.trim="personDraft.name"
            class="w-full min-w-0 rounded border border-gray-300 px-3 py-2"
            placeholder="ชื่อ"
            :readonly="isReadOnly"
            required
          />
          <input
            v-model.trim="personDraft.surname"
            class="w-full min-w-0 rounded border border-gray-300 px-3 py-2"
            placeholder="นามสกุล"
            aria-label="นามสกุล"
            :readonly="isReadOnly"
            required
          />

          <label for="person-birthday" class="font-medium">วันเกิด</label>
          <DatePicker
            id="person-birthday"
            v-model="personDraft.dateOfBirth"
            :max-date="todayDate"
            :readonly="isReadOnly"
          />
          <span />

          <span class="font-medium">อายุ</span>
          <output class="col-span-2">{{ age ?? '—' }} ปี</output>

          <label for="person-address" class="font-medium self-start">ที่อยู่</label>
          <textarea
            id="person-address"
            v-model.trim="personDraft.address"
            class="col-span-2 w-full min-w-0 min-h-24 resize-y rounded border border-gray-300 px-3 py-2"
            placeholder="ที่อยู่"
            :readonly="isReadOnly"
            required
          />
        </div>

        <p v-if="saveError" role="alert" class="text-sm text-red-600">{{ saveError }}</p>

        <div class="flex justify-end gap-3">
          <Button
            type="button"
            class="bg-red-600 text-white hover:bg-red-700"
            @click="isModalOpen = false"
          >
            {{ isReadOnly ? 'ปิด' : 'ยกเลิก' }}
          </Button>
          <Button
            v-if="!isReadOnly"
            type="submit"
            class="bg-cyan-600 text-white hover:bg-cyan-700"
            :disabled="isSaving"
          >
            {{ isSaving ? 'กำลังบันทึก...' : 'บันทึก' }}
          </Button>
        </div>
      </form>
    </DialogContent>
  </Dialog>
</template>

<script setup lang="ts">
import DatePicker from '~/components/ui/date-picker/DatePicker.vue';
import { computed, onMounted, ref } from 'vue';

interface Person {
  id: number;
  name: string;
  surname: string;
  address: string;
  dateOfBirth: string;
  age: number;
}

interface PersonDraft {
  name: string;
  surname: string;
  address: string;
  dateOfBirth: string;
}

const persons = ref<Person[]>([]);
const isModalOpen = ref(false);
const isSaving = ref(false);
const saveError = ref('');
const selectedPersonId = ref<number | null>(null);
const isReadOnly = ref(false);
const personDraft = ref<PersonDraft>(emptyDraft());
const baseUrl = 'http://localhost:5129'; // Replace with your API base URL

const isEditing = computed(() => selectedPersonId.value !== null);
const todayDate = formatInputDate(new Date());
const age = computed(() => calculateAge(personDraft.value.dateOfBirth));

function emptyDraft(): PersonDraft {
  return {
    name: '',
    surname: '',
    address: '',
    dateOfBirth: '',
  };
}

function formatInputDate(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

function toInputDate(date: string): string {
  const parts = date.split('/');
  if (parts.length === 3) {
    const [day, month, year] = parts;
    return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
  }
  return date.slice(0, 10);
}

function formatDisplayDate(date: string): string {
  const [year, month, day] = date.split('-');
  return `${day}/${month}/${year}`;
}

function calculateAge(date: string): number | null {
  if (!date || date > todayDate) return null;

  const [year, month, day] = date.split('-').map(Number);
  const birthDate = new Date(year, month - 1, day);
  if (
    !year
    || !month
    || !day
    || birthDate.getFullYear() !== year
    || birthDate.getMonth() !== month - 1
    || birthDate.getDate() !== day
  ) {
    return null;
  }

  const today = new Date();
  let calculatedAge = today.getFullYear() - year;
  const birthdayThisYear = new Date(today.getFullYear(), month - 1, day);
  if (birthdayThisYear > today) calculatedAge--;
  return calculatedAge;
}

const addPerson = () => {
  selectedPersonId.value = null;
  isReadOnly.value = false;
  personDraft.value = emptyDraft();
  saveError.value = '';
  isModalOpen.value = true;
};

const fetchPersons = async () => {
  const response = await fetch(`${baseUrl}/api/persons`);
  if (!response.ok) {
    throw new Error(`Failed to load people (HTTP ${response.status})`);
  }
  persons.value = await response.json();
};

const openModal = (person: Person) => {
  selectedPersonId.value = person.id;
  isReadOnly.value = true;
  personDraft.value = {
    name: person.name,
    surname: person.surname,
    address: person.address,
    dateOfBirth: toInputDate(person.dateOfBirth),
  };
  saveError.value = '';
  isModalOpen.value = true;
};

const savePerson = async () => {
  if (isReadOnly.value) return;

  if (age.value === null) {
    saveError.value = 'กรุณาเลือกวันเกิดที่ถูกต้อง';
    return;
  }

  isSaving.value = true;
  saveError.value = '';
  const request = {
    ...personDraft.value,
    dateOfBirth: `${personDraft.value.dateOfBirth}T00:00:00`,
  };

  try {
    if (selectedPersonId.value === null) {
      const response = await fetch(`${baseUrl}/api/persons`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(request),
      });
      if (!response.ok) {
        throw new Error(`บันทึกไม่สำเร็จ (HTTP ${response.status})`);
      }
      persons.value.push(await response.json());
    } else {
      const response = await fetch(`${baseUrl}/api/persons/${selectedPersonId.value}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(request),
      });
      if (!response.ok) {
        throw new Error(`บันทึกไม่สำเร็จ (HTTP ${response.status})`);
      }
      const index = persons.value.findIndex(person => person.id === selectedPersonId.value);
      if (index !== -1) {
        persons.value[index] = {
          id: selectedPersonId.value,
          ...personDraft.value,
          dateOfBirth: formatDisplayDate(personDraft.value.dateOfBirth),
          age: age.value,
        };
      }
    }

    isModalOpen.value = false;
  } catch (error) {
    saveError.value = error instanceof Error ? error.message : 'เกิดข้อผิดพลาดขณะบันทึกข้อมูล';
  } finally {
    isSaving.value = false;
  }
};

onMounted(() => {
  fetchPersons().catch((error: unknown) => {
    console.error('Error fetching persons:', error);
  });
});
</script>
