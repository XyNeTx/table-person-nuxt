using System;

namespace modal_view_api.DTO;

public sealed record PersonDto(
    int Id,
    string Name,
    string Surname,
    string Address,
    string DateOfBirth,
    int Age
);