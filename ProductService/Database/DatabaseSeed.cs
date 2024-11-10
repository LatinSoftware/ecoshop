using ProductService.Entities;

namespace ProductService.Database
{
    public class DatabaseSeed
    {
        public static void Seed(ApplicationContext context)
        {
            context.Database.EnsureCreated();

            // populate categories
            if (!context.Categories.Any())
            {
                var categoryList = new List<Category>
                {
                    Category.Create("Electrónica"),
                    Category.Create("Moda"),
                    Category.Create("Hogar y Cocina"),
                    Category.Create("Deportes y Aire Libre"),
                    Category.Create("Juguetes y Juegos"),
                    Category.Create("Belleza y Cuidado Personal"),
                    Category.Create("Libros"),
                };

                context.Categories.AddRange(categoryList);

                var productList = new List<Product>();

                var electronic = categoryList[0];
                productList.Add(Product.Create(electronic.Id, "Teléfono Inteligente Galaxy S21", 799.99m, "Smartphone con pantalla AMOLED de 6.2 pulgadas, cámara de 64MP y 128GB de almacenamiento."));
                productList.Add(Product.Create(electronic.Id, "Auriculares Inalámbricos Bose 700", 299.99m, "Auriculares con cancelación de ruido, Bluetooth y hasta 20 horas de duración de batería."));
                productList.Add(Product.Create(electronic.Id, "Teléfono Inteligente Galaxy S21", 799.99m, "Smartphone con pantalla AMOLED de 6.2 pulgadas, cámara de 64MP y 128GB de almacenamiento."));
                productList.Add(Product.Create(electronic.Id, "Laptop Dell XPS 13", 999.99m, "Smartphone con pantalla AMOLED de 6.2 pulgadas, cámara de 64MP y 128GB de almacenamiento."));

                var moda = categoryList[1];
                productList.Add(Product.Create(moda.Id, "Chaqueta de Cuero para Hombre", 120.00m, "Chaqueta de cuero genuino, ideal para un look casual y elegante."));
                productList.Add(Product.Create(moda.Id, "Zapatillas Deportivas Nike Air Max", 150.00m, "Zapatillas con tecnología de amortiguación Air Max, cómodas y modernas."));
                productList.Add(Product.Create(moda.Id, "Reloj Clásico Casio para Mujer", 799.99m, "Reloj análogo clásico resistente al agua, elegante y minimalista."));

                context.Products.AddRange(productList);

                context.SaveChanges();
            }
        }
    }
}
