using System;

namespace modal_view_api.Models;

public record Person (
    int Id,
    string Name,
    string Surname,
    string Address,
    DateTime DateOfBirth
);
