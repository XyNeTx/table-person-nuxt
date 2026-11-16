using modal_view_api.Models;

namespace modal_view_api;

public static class PersonDbSeeder
{
    public static void Seed(PersonDb db)
    {
        if (db.Person.Any())
        {
            return;
        }

        db.Person.AddRange
        (
            new Person(1, "Alex", "Morgan", "123 Main Street", new DateTime(1990, 5, 12)),
            new Person(2, "Jamie", "Lee", "456 Oak Avenue", new DateTime(1985, 9, 3)),
            new Person(3, "Taylor", "Rivera", "789 Pine Road", new DateTime(2000, 2, 20)),
            new Person(4, "Jordan", "Kim", "14 Cedar Lane", new DateTime(1995, 7, 8)),
            new Person(5, "Morgan", "Chen", "82 Lakeview Drive", new DateTime(1988, 11, 2)),
            new Person(6, "Casey", "Patel", "305 Birch Street", new DateTime(2002, 3, 17)),
            new Person(7, "Riley", "Thompson", "9 Willow Court", new DateTime(1992, 1, 25)),
            new Person(8, "Avery", "Nguyen", "61 Sunset Boulevard", new DateTime(1998, 10, 14)),
            new Person(9, "Quinn", "Garcia", "27 Maple Avenue", new DateTime(1982, 6, 30)),
            new Person(10, "Drew", "Wilson", "410 Park Road", new DateTime(2005, 12, 4)),
            new Person(11, "Skyler", "Brown", "73 Highland Way", new DateTime(1991, 4, 19)),
            new Person(12, "Reese", "Davis", "156 Meadow Lane", new DateTime(1987, 8, 11)),
            new Person(13, "Cameron", "Singh", "33 River Street", new DateTime(1999, 2, 6)),
            new Person(14, "Parker", "Johnson", "208 Forest Avenue", new DateTime(1994, 9, 22)),
            new Person(15, "Hayden", "Lopez", "95 Orchard Road", new DateTime(2001, 5, 29))
        );

        db.SaveChanges();
    }
}
