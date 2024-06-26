﻿namespace VicStelmak.SMA.WebUI.Product.Dtos
{
    public record UpdateProductDto(
     int AmountInStock,
     string UpdatedBy,
     string Description,
     string ImageUri,
     string Name,
     decimal Price);
}
