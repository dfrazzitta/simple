using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Bogus.DataSets;
using Bogus.Extensions;


namespace Simple.Models
{
    public enum Gender
    {
        Male,
        Female,
    }

    public static class GenderExtensions
    {
        public static Name.Gender MapToLibGender(this Gender gender)
        {
            return gender == Gender.Male ? Name.Gender.Male : Name.Gender.Female;
        }
    }
}


namespace Simple.Models
{
    public class RandomData
    {
        public static List<string> Activities = new List<string>()
        {
            "Baking","Blogging", "Bowling","Collecting", "Hacking", "Backpacking",
            "Canyoning", "Geocaching", "Orienteering", "Golf","Climbing", "Running", "Snow-kiting",
            "Horseback riding", "Photography", "Scuba diving","Wildlife watching",
            "Wine tourism", "Road Touring", "Reading"
        };

        public static List<string> AvailableSports = new List<string>()
        {
            "Soccer","Basketball", "Tennis","Volleyball", "Beach Volleyball", "American Football",
            "Baseball", "Ice Hockey", "Formula 1", "Moto GP","Motor Sport", "Handball", "Water Polo",
            "Table Tennis", "Darts","Snooker", "MMA", "Boxing","Cricket", "Cycling", "Golf"
        };

        public static List<string> AvailableProfessions = new List<string>()
        {
            "Dentist","Photographer","Pharmacist","Teacher","Flight Attendant","Founder / Entrepreneur",
            "Personal Trainer","Waitress / Bartender","Physical Therapist","Lawyer","Marketing Manager","Pilot",
            "Producer","Visual Designer","Model","Engineer", "Firefighter","Doctor","Financial Adviser", "Police Officer",
            "Social-Media Manager","Nurse","Real-Estate Agent"
        };
        public static List<User> GenerateUsers(int count, string locale = "en")
        {
            var person = new Faker<User>(locale)
                .RuleFor(u => u.Gender, f => f.PickRandom<Gender>())
                .RuleFor(u => u.FirstName, (f, u) =>
                    f.Name.FirstName(u.Gender.MapToLibGender())) 
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(u.Gender.MapToLibGender()))
                .RuleFor(u => u.UserName, (f, u) => f.Internet.UserName(u.FirstName, u.LastName))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.Avatar, f => f.Internet.Avatar())
                .RuleFor(u => u.DateOfBirth, (f, u) => f.Date.Past(50, new DateTime?(Date.SystemClock().AddYears(-20))))
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Website, f => f.Internet.DomainName().OrNull(f, 0.1f))
               // .RuleFor(u => u.Address, f => GenerateCardAddress(locale))
               /*
                .RuleFor(u => u.Company, (f, u) => new CompanyCard()
                {
                    Name = f.Company.CompanyName(new int?()),
                    CatchPhrase = f.Company.CatchPhrase(),
                    Bs = f.Company.Bs()
                })
               */
                .RuleFor(u => u.Salary, (f, u) => Math.Round(f.Finance.Amount(1000, 5000)))
                .RuleFor(u => u.MonthlyExpenses, (f, u) => f.Random.Number(1500, 6500))
                .RuleFor(u => u.FavoriteSports,
                    (f, u) => f.PickRandom(AvailableSports, f.Random.Number(1, 19)).ToList())
                .RuleFor(u => u.Profession, (f, u) => f.PickRandom(AvailableProfessions, 1).FirstOrDefault());

            return person.Generate(count);
        }

        public static List<Order> GenerateOrders(int count, string locale = "en")
        {
            var orderIds = 0;
            var order = new Faker<Order>(locale)
                .StrictMode(true)
                .RuleFor(o => o.OrderId, f => orderIds++)
                .RuleFor(o => o.Item, f => f.Commerce.ProductName())
                .RuleFor(o => o.Price, f => f.Random.Int(1, 300))
                .RuleFor(o => o.Quantity, f => f.Random.Number(1, 10))
                .RuleFor(o => o.LotNumber, f => f.Random.Int(0, 100).OrNull(f, .8f))
                .RuleFor(o => o.ShipmentDetails, f => GenerateShipmentDetails(locale));

            return order.Generate(count);

        }

        public static List<Product> GenerateProducts(int count, string locale = "en")
        {
            var product = new Faker<Product>(locale)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName());

            return product.Generate(count);

        }

        public static ShipmentDetails GenerateShipmentDetails(string locale = "en")
        {
            var shipmentDetails = new Faker<ShipmentDetails>(locale)
                .RuleFor(u => u.ContactName, (f, u) => f.Name.FullName())
                .RuleFor(u => u.ContactPhone, (f, u) => f.Phone.PhoneNumber().OrNull(f, .3f))
                .RuleFor(u => u.City, (f, u) => f.Address.City())
                .RuleFor(u => u.ShipAddress, (f, u) => f.Address.FullAddress())
                .RuleFor(u => u.Country, f => f.Address.Country())
                .RuleFor(u => u.ShippedDate, (f, u) =>
                    f.Date.Between(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow).OrNull(f, .8f));

            return shipmentDetails.Generate();
        }




 
 
 
    }
}
