using System;

namespace modal_view_api.DTO;

public sealed record PersonRequest(
    string Name,
    string Surname,
    string Address,
    DateTime DateOfBirth
);