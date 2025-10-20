namespace dacn_api.Models.DTOs
{
    public class FoodDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Barcode { get; set; }
        public double Calories { get; set; }
        public double? Protein { get; set; }
        public double? Carbs { get; set; }
        public double? Fat { get; set; }
        public string? ServingSize { get; set; }
        public string? Type { get; set; }
        public string? Instructions { get; set; }
    }

    public class CreateFoodDto
    {
        public string Name { get; set; } = null!;
        public string? Barcode { get; set; }
        public double Calories { get; set; }
        public double? Protein { get; set; }
        public double? Carbs { get; set; }
        public double? Fat { get; set; }
        public string? ServingSize { get; set; }
        public string? Type { get; set; }
        public string? Instructions { get; set; }
    }

    public class MealItemDto
    {
        public int Id { get; set; }
        public int? FoodId { get; set; }
        public string? FoodName { get; set; }
        public double Quantity { get; set; }
        public string? Unit { get; set; }
        public double? Calories { get; set; }
        public double? Protein { get; set; }
        public double? Carbs { get; set; }
        public double? Fat { get; set; }
    }

    public class CreateMealItemDto
    {
        public int? FoodId { get; set; }
        public double Quantity { get; set; }
        public string? Unit { get; set; }
        public double? Calories { get; set; }
        public double? Protein { get; set; }
        public double? Carbs { get; set; }
        public double? Fat { get; set; }
    }

    public class MealRecordDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string MealType { get; set; } = null!;
        public double? TotalCalories { get; set; }
        public string? Note { get; set; }
        public List<MealItemDto>? Items { get; set; }
    }

    public class CreateMealRecordDto
    {
        public DateOnly Date { get; set; }
        public string MealType { get; set; } = null!;
        public string? Note { get; set; }
        public List<CreateMealItemDto>? Items { get; set; }
    }
}
