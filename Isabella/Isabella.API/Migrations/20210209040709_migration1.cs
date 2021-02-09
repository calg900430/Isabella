using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Isabella.API.Migrations
{
    public partial class migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    IdForClaim = table.Column<Guid>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    LastDateConnected = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    RecoverPassword = table.Column<bool>(nullable: false),
                    Enable = table.Column<bool>(nullable: false),
                    ImageUserProfile = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodeIdentifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeIdentifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude_Gps = table.Column<double>(nullable: false),
                    Longitude_Gps = table.Column<double>(nullable: false),
                    Favorite_Gps = table.Column<int>(nullable: false),
                    Name_Gps = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CalificationRestaurants",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Calification = table.Column<int>(nullable: false),
                    Opinion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalificationRestaurants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalificationRestaurants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmationsEmails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationsEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfirmationsEmails_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecoverPassword",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoverPassword", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecoverPassword_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAggregates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    LastBuy = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdate = table.Column<DateTime>(nullable: true),
                    IsAvailabe = table.Column<bool>(nullable: false),
                    Stock = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Average = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAggregates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAggregates_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductsSpecials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    LastBuy = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdate = table.Column<DateTime>(nullable: true),
                    IsAvailabe = table.Column<bool>(nullable: false),
                    Stock = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Average = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsSpecials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsSpecials_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductsStandards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    LastBuy = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdate = table.Column<DateTime>(nullable: true),
                    IsAvailabe = table.Column<bool>(nullable: false),
                    Stock = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Average = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsStandards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsStandards_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeVerificationId = table.Column<int>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    GpsId = table.Column<int>(nullable: true),
                    Address = table.Column<string>(nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    DeliveryDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_CodeIdentifications_CodeVerificationId",
                        column: x => x.CodeVerificationId,
                        principalTable: "CodeIdentifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Gps_GpsId",
                        column: x => x.GpsId,
                        principalTable: "Gps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImageProductAggregates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductAggregateId = table.Column<int>(nullable: true),
                    Image = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageProductAggregates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageProductAggregates_ProductAggregates_ProductAggregateId",
                        column: x => x.ProductAggregateId,
                        principalTable: "ProductAggregates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalificationProductSpecials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Calification = table.Column<int>(nullable: false),
                    Opinion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalificationProductSpecials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalificationProductSpecials_ProductsSpecials_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ProductsSpecials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalificationProductSpecials_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarShopsProductsSpecials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeIdentificationId = table.Column<int>(nullable: true),
                    ProductSpecialId = table.Column<int>(nullable: true),
                    CheeseGouda = table.Column<bool>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarShopsProductsSpecials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarShopsProductsSpecials_CodeIdentifications_CodeIdentificationId",
                        column: x => x.CodeIdentificationId,
                        principalTable: "CodeIdentifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarShopsProductsSpecials_ProductsSpecials_ProductSpecialId",
                        column: x => x.ProductSpecialId,
                        principalTable: "ProductsSpecials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImageProductSpecials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductSpecialId = table.Column<int>(nullable: true),
                    Image = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageProductSpecials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageProductSpecials_ProductsSpecials_ProductSpecialId",
                        column: x => x.ProductSpecialId,
                        principalTable: "ProductsSpecials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubCategory_ProductSpecials",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductSpecialId = table.Column<int>(nullable: true),
                    SubCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategory_ProductSpecials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategory_ProductSpecials_ProductsSpecials_ProductSpecialId",
                        column: x => x.ProductSpecialId,
                        principalTable: "ProductsSpecials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubCategory_ProductSpecials_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CalificationProductStandards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    Calification = table.Column<int>(nullable: false),
                    Opinion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalificationProductStandards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalificationProductStandards_ProductsStandards_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ProductsStandards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalificationProductStandards_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarShopsProductsStandards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodeIdentificationId = table.Column<int>(nullable: true),
                    ProductStandardId = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarShopsProductsStandards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarShopsProductsStandards_CodeIdentifications_CodeIdentificationId",
                        column: x => x.CodeIdentificationId,
                        principalTable: "CodeIdentifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarShopsProductsStandards_ProductsStandards_ProductStandardId",
                        column: x => x.ProductStandardId,
                        principalTable: "ProductsStandards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImageProductStandards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductStandardId = table.Column<int>(nullable: true),
                    Image = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageProductStandards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageProductStandards_ProductsStandards_ProductStandardId",
                        column: x => x.ProductStandardId,
                        principalTable: "ProductsStandards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubCategory_ProductStandards",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductStandardId = table.Column<int>(nullable: true),
                    SubCategoryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategory_ProductStandards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategory_ProductStandards_ProductsStandards_ProductStandardId",
                        column: x => x.ProductStandardId,
                        principalTable: "ProductsStandards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubCategory_ProductStandards_SubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarShopProductAggregates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarShopProductSpecialId = table.Column<int>(nullable: true),
                    ProductAggregateId = table.Column<int>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarShopProductAggregates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarShopProductAggregates_CarShopsProductsSpecials_CarShopProductSpecialId",
                        column: x => x.CarShopProductSpecialId,
                        principalTable: "CarShopsProductsSpecials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarShopProductAggregates_ProductAggregates_ProductAggregateId",
                        column: x => x.ProductAggregateId,
                        principalTable: "ProductAggregates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CalificationProductSpecials_ProductId",
                table: "CalificationProductSpecials",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CalificationProductSpecials_UserId",
                table: "CalificationProductSpecials",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CalificationProductStandards_ProductId",
                table: "CalificationProductStandards",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CalificationProductStandards_UserId",
                table: "CalificationProductStandards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CalificationRestaurants_UserId",
                table: "CalificationRestaurants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarShopProductAggregates_CarShopProductSpecialId",
                table: "CarShopProductAggregates",
                column: "CarShopProductSpecialId");

            migrationBuilder.CreateIndex(
                name: "IX_CarShopProductAggregates_ProductAggregateId",
                table: "CarShopProductAggregates",
                column: "ProductAggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_CarShopsProductsSpecials_CodeIdentificationId",
                table: "CarShopsProductsSpecials",
                column: "CodeIdentificationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarShopsProductsSpecials_ProductSpecialId",
                table: "CarShopsProductsSpecials",
                column: "ProductSpecialId");

            migrationBuilder.CreateIndex(
                name: "IX_CarShopsProductsStandards_CodeIdentificationId",
                table: "CarShopsProductsStandards",
                column: "CodeIdentificationId");

            migrationBuilder.CreateIndex(
                name: "IX_CarShopsProductsStandards_ProductStandardId",
                table: "CarShopsProductsStandards",
                column: "ProductStandardId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationsEmails_UserId",
                table: "ConfirmationsEmails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageProductAggregates_ProductAggregateId",
                table: "ImageProductAggregates",
                column: "ProductAggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageProductSpecials_ProductSpecialId",
                table: "ImageProductSpecials",
                column: "ProductSpecialId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageProductStandards_ProductStandardId",
                table: "ImageProductStandards",
                column: "ProductStandardId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CodeVerificationId",
                table: "Orders",
                column: "CodeVerificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_GpsId",
                table: "Orders",
                column: "GpsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAggregates_CategoryId",
                table: "ProductAggregates",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsSpecials_CategoryId",
                table: "ProductsSpecials",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsStandards_CategoryId",
                table: "ProductsStandards",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecoverPassword_UserId",
                table: "RecoverPassword",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_ProductSpecials_ProductSpecialId",
                table: "SubCategory_ProductSpecials",
                column: "ProductSpecialId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_ProductSpecials_SubCategoryId",
                table: "SubCategory_ProductSpecials",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_ProductStandards_ProductStandardId",
                table: "SubCategory_ProductStandards",
                column: "ProductStandardId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_ProductStandards_SubCategoryId",
                table: "SubCategory_ProductStandards",
                column: "SubCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CalificationProductSpecials");

            migrationBuilder.DropTable(
                name: "CalificationProductStandards");

            migrationBuilder.DropTable(
                name: "CalificationRestaurants");

            migrationBuilder.DropTable(
                name: "CarShopProductAggregates");

            migrationBuilder.DropTable(
                name: "CarShopsProductsStandards");

            migrationBuilder.DropTable(
                name: "ConfirmationsEmails");

            migrationBuilder.DropTable(
                name: "ImageProductAggregates");

            migrationBuilder.DropTable(
                name: "ImageProductSpecials");

            migrationBuilder.DropTable(
                name: "ImageProductStandards");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "RecoverPassword");

            migrationBuilder.DropTable(
                name: "SubCategory_ProductSpecials");

            migrationBuilder.DropTable(
                name: "SubCategory_ProductStandards");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CarShopsProductsSpecials");

            migrationBuilder.DropTable(
                name: "ProductAggregates");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ProductsStandards");

            migrationBuilder.DropTable(
                name: "SubCategories");

            migrationBuilder.DropTable(
                name: "ProductsSpecials");

            migrationBuilder.DropTable(
                name: "CodeIdentifications");

            migrationBuilder.DropTable(
                name: "Gps");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
