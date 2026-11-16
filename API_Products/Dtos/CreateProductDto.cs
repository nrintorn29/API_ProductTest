using System.ComponentModel.DataAnnotations;

public class CreateProductDto
{
    [Required(ErrorMessage = "กรุณาระบุชื่อสินค้า")]
    [StringLength(100, ErrorMessage = "ชื่อสินค้าไม่เกิน 100 ตัวอักษร")]
    public string Name {get; set;} = string.Empty;

    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "ราคาระหว่าง 0.01 ถึง 1000000")]
    public decimal Price {get; set;}

    [StringLength(500, ErrorMessage = "ข้อความไม่เกิน 500 ตัวอักษร")]
    public string? Description {get; set;}
}