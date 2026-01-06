using Infastructure.Data;
using Microsoft.AspNetCore.Identity;
using Models;

namespace Infrastructure.Seed;

public class Seeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public Seeder(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> SeedRole()
    {
        var newroles = new List<IdentityRole<int>>()
        {
            new IdentityRole<int>(Roles.Admin),
            new IdentityRole<int>(Roles.Moderator),
            new IdentityRole<int>(Roles.User)
        };

        var existing = _roleManager.Roles.ToList();
        foreach (var role in newroles)
        {
            if (existing.Exists(e => e.Name == role.Name) == false)
            {
                await _roleManager.CreateAsync(role);
            }
        }

        return true;

    }

    public async Task<bool> SeedUser()
    {
        var existing = await _userManager.FindByNameAsync("admin");
        if (existing != null) return false;
        
        var identity = new User()
        {
            UserName = "admin",
            PhoneNumber = "13456777",
            Email = "admin@gmail.com",
            Name = "Admin First"
        };

        var result = await _userManager.CreateAsync(identity, "hello123");
        await _userManager.AddToRoleAsync(identity, Roles.Admin);
        return result.Succeeded;
    }
    
    public async Task<bool> SeedRealEstateCategories()
    {
        const string mainName = "Недвижимость";
        if (_context.MainCategories.Any(m => m.Name == mainName)) return false;

        var main = new MainCategory
        {
            Name = mainName,
            Subcategories = new List<Subcategory>()
        };

        var subs = new List<Subcategory>
        {
            new Subcategory { Title = "Аренда комнат", Section = RealEstateSection.Rent, Category = RealEstateCategory.Room },
            new Subcategory { Title = "Аренда квартир", Section = RealEstateSection.Rent, Category = RealEstateCategory.Apartment },
            new Subcategory { Title = "Посуточная аренда квартир, домов", Section = RealEstateSection.DailyRent, Category = RealEstateCategory.Apartment },
            new Subcategory { Title = "Аренда домов (хавли)", Section = RealEstateSection.Rent, Category = RealEstateCategory.HouseOrDacha },
            new Subcategory { Title = "Аренда дач", Section = RealEstateSection.Rent, Category = RealEstateCategory.HouseOrDacha },
            new Subcategory { Title = "Аренда офисов и помещений", Section = RealEstateSection.Rent, Category = RealEstateCategory.OfficeOrPremises },
            new Subcategory { Title = "Продажа квартир", Section = RealEstateSection.Sale, Category = RealEstateCategory.Apartment },
            new Subcategory { Title = "Продажа домов (хавли) и дач", Section = RealEstateSection.Sale, Category = RealEstateCategory.HouseOrDacha },
            new Subcategory { Title = "Продажа, аренда построек с земельным участком", Section = RealEstateSection.Sale, Category = RealEstateCategory.PlotWithBuilding },
            new Subcategory { Title = "Продажа, аренда гаражей и стоянок", Section = RealEstateSection.Sale, Category = RealEstateCategory.GarageOrParking },
            new Subcategory { Title = "Продажа офисов и помещений", Section = RealEstateSection.Sale, Category = RealEstateCategory.OfficeOrPremises },
            new Subcategory { Title = "Продажа отдельно стоящих зданий", Section = RealEstateSection.Sale, Category = RealEstateCategory.StandaloneBuilding },
            new Subcategory { Title = "Продажа, аренда вагончиков, бытовок, вагон-домов", Section = RealEstateSection.Sale, Category = RealEstateCategory.CabinOrContainer }
        };

        foreach (var s in subs) main.Subcategories.Add(s);

        _context.MainCategories.Add(main);
        await _context.SaveChangesAsync();
        return true;
    }
    
   

}

public class Roles
{
    public const string Admin = "Admin";
    public const string Moderator = "Moderator";
    public const string User = "User";
}

