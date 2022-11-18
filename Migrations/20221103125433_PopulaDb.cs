using Microsoft.EntityFrameworkCore.Migrations;

namespace APICatalogo.Migrations
{
    public partial class PopulaDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES ('Bebidas', 'bebidas.jpg')");
            migrationBuilder.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES ('Lanches', 'lanches.jpg')");
            migrationBuilder.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES ('Sobremesas', 'sobremesas.jpg')");

            migrationBuilder.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " +
                "VALUES ('Coca-Cola Diet', 'Refrigerante de Cola 450ml', '6.99', 'coca-cola.jpg', '30', now(), (select CategoriaId from Categorias where nome='Bebidas'))");

            migrationBuilder.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " +
                "VALUES ('Sanduíche de Frango', 'Sanduíche natural de frango', '9.99', 'sanduíche-frango.jpg', '32', now(), (select CategoriaId from Categorias where nome='Lanches'))");

            migrationBuilder.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId) " +
                "VALUES ('Brigadeiro de Chocolate', 'Brigadeiro 50g', '0.60', 'brigadeiro.jpg', '30', now(), (select CategoriaId from Categorias where nome='Sobremesas'))");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Categorias");
            migrationBuilder.Sql("DELETE FROM Produtos");
        }
    }
}
