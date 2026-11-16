using System;
using Microsoft.EntityFrameworkCore;
using modal_view_api.DTO;
using modal_view_api.helpers;
using modal_view_api.Models;

namespace modal_view_api;

public static class PersonEndpoint
{
    public static void MapPersonEndpoints(this WebApplication app)
    {
        app.MapGet("/api/persons", async (PersonDb db) =>
        {
            var persons = await db.Person.ToListAsync();
            var personDtos = persons.Select(p => new PersonDto(
                p.Id,
                p.Name,
                p.Surname,
                p.Address,
                p.DateOfBirth.ToString("dd/MM/yyyy"),
                CalAge.CalculateAge(p.DateOfBirth)
            )).ToList();
            return Results.Ok(personDtos);
        });

        app.MapGet("/api/persons/{id}", async (int id, PersonDb db) =>
        {
            var person = await db.Person.FindAsync(id);
            return person is not null
                ? Results.Ok(new PersonDto(
                    person.Id,
                    person.Name,
                    person.Surname,
                    person.Address,
                    person.DateOfBirth.ToString("dd/MM/yyyy"),
                    CalAge.CalculateAge(person.DateOfBirth)))
                : Results.NotFound();
        });

        app.MapPost("/api/persons", async (PersonRequest request, PersonDb db) =>
        {
            var person = new Person(
                0,
                request.Name,
                request.Surname,
                request.Address,
                request.DateOfBirth);
            db.Person.Add(person);
            await db.SaveChangesAsync();
            return Results.Created(
                $"/api/persons/{person.Id}",
                new PersonDto(
                    person.Id,
                    person.Name,
                    person.Surname,
                    person.Address,
                    person.DateOfBirth.ToString("dd/MM/yyyy"),
                    CalAge.CalculateAge(person.DateOfBirth)));
        });

        app.MapPut("/api/persons/{id}", async (int id, PersonRequest request, PersonDb db) =>
        {
            var existingPerson = await db.Person.FindAsync(id);
            if (existingPerson is null)
            {
                return Results.NotFound();
            }

            var updatedPerson = new Person(
                id,
                request.Name,
                request.Surname,
                request.Address,
                request.DateOfBirth);
            db.Entry(existingPerson).CurrentValues.SetValues(updatedPerson);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        app.MapDelete("/api/persons/{id}", async (int id, PersonDb db) =>
        {
            var person = await db.Person.FindAsync(id);
            if (person is null)
            {
                return Results.NotFound();
            }

            db.Person.Remove(person);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}
