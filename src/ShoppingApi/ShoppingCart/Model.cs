using ShoppingApi.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingApi.ShoppingCart
{
    public class ProductDTO
    {
        [NotEmptyGuid]
        public Guid Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Brand { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid quantity number")]
        public int Quantity { get; set; }
        [Range(0.1, float.MaxValue, ErrorMessage = "Please enter valid amount")]
        public float Amount { get; set; }
        [Range(0.1, float.MaxValue, ErrorMessage = "Please enter valid deci")]
        public double Deci { get; set; }
    }

    public class CartDTO
    {
        [NotEmptyGuid]
        public Guid Id { get; set; }
        public List<ProductDTO> Products { get; set; }
    }
}
