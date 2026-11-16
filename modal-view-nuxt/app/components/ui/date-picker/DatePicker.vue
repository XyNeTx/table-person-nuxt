<script setup lang="ts">
import type { DateValue } from "@internationalized/date"
import { CalendarDate, parseDate } from "@internationalized/date"
import { CalendarDays, ChevronLeft, ChevronRight } from "@lucide/vue"
import {
  CalendarCell,
  CalendarCellTrigger,
  CalendarGrid,
  CalendarGridBody,
  CalendarGridHead,
  CalendarGridRow,
  CalendarHeadCell,
  CalendarHeader,
  CalendarNext,
  CalendarPrev,
  CalendarRoot,
  PopoverContent,
  PopoverPortal,
  PopoverRoot,
  PopoverTrigger,
} from "reka-ui"
import { computed, ref, watch } from "vue"

const props = defineProps<{
  modelValue: string
  maxDate: string
  readonly?: boolean
  id?: string
}>()
const emit = defineEmits<{
  "update:modelValue": [value: string]
}>()

const isOpen = ref(false)
const selectedDate = computed(() => props.modelValue ? parseDate(props.modelValue) : undefined)
const maximumDate = computed(() => parseDate(props.maxDate))
const placeholderDate = ref<DateValue>(selectedDate.value ?? maximumDate.value)
const currentYear = maximumDate.value.year
const years = Array.from({ length: currentYear - 1899 }, (_, index) => currentYear - index)
const months = Array.from({ length: 12 }, (_, index) => ({
  value: index + 1,
  label: new Date(2000, index).toLocaleString("en-GB", { month: "long" }),
}))
const selectedYear = computed({
  get: () => placeholderDate.value.year,
  set: (year: number) => {
    placeholderDate.value = new CalendarDate(year, placeholderDate.value.month, 1)
  },
})
const selectedMonth = computed({
  get: () => placeholderDate.value.month,
  set: (month: number) => {
    placeholderDate.value = new CalendarDate(placeholderDate.value.year, month, 1)
  },
})
const displayDate = computed(() => {
  if (!selectedDate.value) return "เลือกวันเกิด"

  const { day, month, year } = selectedDate.value
  return `${String(day).padStart(2, "0")}/${String(month).padStart(2, "0")}/${year}`
})

watch(selectedDate, (date) => {
  if (date) placeholderDate.value = date
})

function selectDate(value: DateValue | DateValue[] | undefined) {
  if (props.readonly || !value || Array.isArray(value)) return

  emit("update:modelValue", value.toString())
  isOpen.value = false
}

</script>

<template>
  <PopoverRoot v-model:open="isOpen">
    <PopoverTrigger as-child>
      <button
        :id="id"
        type="button"
        :disabled="readonly"
        class="flex h-10 w-full min-w-0 items-center justify-between gap-2 rounded-md border border-input bg-background px-3 py-2 text-left text-sm shadow-xs outline-none transition-colors hover:bg-accent hover:text-accent-foreground focus-visible:border-ring focus-visible:ring-3 focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-70"
        :aria-label="selectedDate ? `วันเกิด ${displayDate}` : 'เลือกวันเกิด'"
      >
        <span :class="selectedDate ? '' : 'text-muted-foreground'">{{ displayDate }}</span>
        <CalendarDays class="size-4 shrink-0 opacity-60" />
      </button>
    </PopoverTrigger>
    <PopoverPortal>
      <PopoverContent
        align="start"
        :side-offset="4"
        class="z-[100] w-auto rounded-md border bg-popover p-3 text-popover-foreground shadow-md"
      >
        <CalendarRoot
          :model-value="selectedDate"
          v-model:placeholder="placeholderDate"
          :max-value="maximumDate"
          :week-starts-on="1"
          locale="en-GB"
          initial-focus
          @update:model-value="selectDate"
          v-slot="{ weekDays, grid }"
        >
          <CalendarHeader class="relative flex w-full items-center justify-center px-8 pt-1">
            <CalendarPrev
              class="absolute left-1 inline-flex size-7 items-center justify-center rounded-md bg-transparent p-0 opacity-70 hover:bg-accent hover:opacity-100"
            >
              <ChevronLeft class="size-4" />
            </CalendarPrev>
            <div class="flex items-center gap-1 text-sm font-medium">
              <select
                v-model.number="selectedMonth"
                aria-label="เลือกเดือน"
                class="rounded-md border-0 bg-transparent px-1 py-1 outline-none focus-visible:ring-2 focus-visible:ring-ring/50"
              >
                <option v-for="month in months" :key="month.value" :value="month.value">{{ month.label }}</option>
              </select>
              <select
                v-model.number="selectedYear"
                aria-label="เลือกปี"
                class="rounded-md border-0 bg-transparent px-1 py-1 outline-none focus-visible:ring-2 focus-visible:ring-ring/50"
              >
                <option v-for="year in years" :key="year" :value="year">{{ year }}</option>
              </select>
            </div>
            <CalendarNext
              class="absolute right-1 inline-flex size-7 items-center justify-center rounded-md bg-transparent p-0 opacity-70 hover:bg-accent hover:opacity-100"
            >
              <ChevronRight class="size-4" />
            </CalendarNext>
          </CalendarHeader>

          <div class="mt-4 space-y-4">
            <CalendarGrid
              v-for="monthGrid in grid"
              :key="monthGrid.value.toString()"
              class="w-full border-collapse"
            >
              <CalendarGridHead>
                <CalendarGridRow class="flex">
                  <CalendarHeadCell
                    v-for="day in weekDays"
                    :key="day"
                    class="w-9 rounded-md text-center text-[0.8rem] font-normal text-muted-foreground"
                  >
                    {{ day }}
                  </CalendarHeadCell>
                </CalendarGridRow>
              </CalendarGridHead>
              <CalendarGridBody>
                <CalendarGridRow
                  v-for="(week, weekIndex) in monthGrid.rows"
                  :key="weekIndex"
                  class="mt-2 flex w-full"
                >
                  <CalendarCell
                    v-for="day in week"
                    :key="day.toString()"
                    :date="day"
                    class="relative p-0 text-center text-sm"
                  >
                    <CalendarCellTrigger
                      :day="day"
                      :month="monthGrid.value"
                      as="button"
                      class="inline-flex size-9 items-center justify-center rounded-md p-0 font-normal outline-none transition-colors hover:bg-accent hover:text-accent-foreground focus-visible:ring-2 focus-visible:ring-ring/50 data-[selected]:bg-primary data-[selected]:text-primary-foreground data-[disabled]:pointer-events-none data-[disabled]:opacity-40 data-[outside-view]:text-muted-foreground/50"
                    />
                  </CalendarCell>
                </CalendarGridRow>
              </CalendarGridBody>
            </CalendarGrid>
          </div>
        </CalendarRoot>
      </PopoverContent>
    </PopoverPortal>
  </PopoverRoot>
</template>
