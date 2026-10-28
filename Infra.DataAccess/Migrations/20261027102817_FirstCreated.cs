using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infra.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FirstCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RssSiteTable",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    SiteName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RssSiteTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RssFeedTable",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    RssSiteEfEntityId = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RssFeedTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RssFeedTable_RssSiteTable_RssSiteEfEntityId",
                        column: x => x.RssSiteEfEntityId,
                        principalTable: "RssSiteTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RssFeedXmlParsingConfigTable",
                columns: table => new
                {
                    RssFeedEfEntityId = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ParentElementName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    ChildElementNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributeNames = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RssFeedXmlParsingConfigTable", x => x.RssFeedEfEntityId);
                    table.ForeignKey(
                        name: "FK_RssFeedXmlParsingConfigTable_RssFeedTable_RssFeedEfEntityId",
                        column: x => x.RssFeedEfEntityId,
                        principalTable: "RssFeedTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RssItemFieldKeyConfigTable",
                columns: table => new
                {
                    RssFeedEfEntityId = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    IdKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    LinkKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CategoryKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TitleKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DescriptionKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PublishDateKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Argb8BitImageKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RssItemFieldKeyConfigTable", x => x.RssFeedEfEntityId);
                    table.ForeignKey(
                        name: "FK_RssItemFieldKeyConfigTable_RssFeedTable_RssFeedEfEntityId",
                        column: x => x.RssFeedEfEntityId,
                        principalTable: "RssFeedTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RssItemTable",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    RssFeedEfEntityId = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageFullFilePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RssItemTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RssItemTable_RssFeedTable_RssFeedEfEntityId",
                        column: x => x.RssFeedEfEntityId,
                        principalTable: "RssFeedTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "RssSiteTable",
                columns: new[] { "Id", "SiteName" },
                values: new object[,]
                {
                    { "http://www.tasnimnews.ir/", "خبرگزاری تسنیم" },
                    { "https://www.yjc.ir/", "باشگاه خبرنگاران جوان" }
                });

            migrationBuilder.InsertData(
                table: "RssFeedTable",
                columns: new[] { "Id", "RssSiteEfEntityId", "Title" },
                values: new object[,]
                {
                    { "https://www.tasnimnews.ir/fa/rss/feed/0/0/8/0/AllStories", "http://www.tasnimnews.ir/", "صفحه اصلی" },
                    { "https://www.tasnimnews.ir/fa/rss/feed/0/0/8/1/TopStories", "http://www.tasnimnews.ir/", "آخرین خبرهای روز" },
                    { "https://www.tasnimnews.ir/fa/rss/feed/1127/0/7/0/MostPupolar", "http://www.tasnimnews.ir/", "پر بیننده ترین اخبار امام و رهبری" },
                    { "https://www.tasnimnews.ir/fa/rss/feeds/1/0/0/0", "http://www.tasnimnews.ir/", "سیاسی" },
                    { "https://www.tasnimnews.ir/fa/rss/feeds/4/0/0/0", "http://www.tasnimnews.ir/", "فرهنگی" },
                    { "https://www.tasnimnews.ir/fa/rss/feeds/8/0/0/0", "http://www.tasnimnews.ir/", "بین الملل" },
                    { "https://www.yjc.ir/fa/rss/11", "https://www.yjc.ir/", "فیلم و صوت" },
                    { "https://www.yjc.ir/fa/rss/3", "https://www.yjc.ir/", "سیاسی" },
                    { "https://www.yjc.ir/fa/rss/4", "https://www.yjc.ir/", "فرهنگی هنری" },
                    { "https://www.yjc.ir/fa/rss/6", "https://www.yjc.ir/", "اقتصادی" },
                    { "https://www.yjc.ir/fa/rss/7", "https://www.yjc.ir/", "علمی پزشکی" },
                    { "https://www.yjc.ir/fa/rss/9", "https://www.yjc.ir/", "بین الملل" },
                    { "https://www.yjc.ir/fa/rss/allnews", "https://www.yjc.ir/", "کل اخبار" }
                });

            migrationBuilder.InsertData(
                table: "RssFeedXmlParsingConfigTable",
                columns: new[] { "RssFeedEfEntityId", "AttributeNames", "ChildElementNames", "ParentElementName" },
                values: new object[,]
                {
                    { "https://www.tasnimnews.ir/fa/rss/feed/0/0/8/0/AllStories", "[\"url\"]", "[\"guid\",\"link\",\"category\",\"title\",\"description\",\"pubDate\",\"{http://search.yahoo.com/mrss/}thumbnail\"]", "item" },
                    { "https://www.tasnimnews.ir/fa/rss/feed/0/0/8/1/TopStories", "[\"url\"]", "[\"guid\",\"link\",\"category\",\"title\",\"description\",\"pubDate\",\"{http://search.yahoo.com/mrss/}thumbnail\"]", "item" },
                    { "https://www.tasnimnews.ir/fa/rss/feed/1127/0/7/0/MostPupolar", "[\"url\"]", "[\"guid\",\"link\",\"category\",\"title\",\"description\",\"pubDate\",\"{http://search.yahoo.com/mrss/}thumbnail\"]", "item" },
                    { "https://www.tasnimnews.ir/fa/rss/feeds/1/0/0/0", "[\"url\"]", "[\"guid\",\"link\",\"category\",\"title\",\"description\",\"pubDate\",\"{http://search.yahoo.com/mrss/}thumbnail\"]", "item" },
                    { "https://www.tasnimnews.ir/fa/rss/feeds/4/0/0/0", "[\"url\"]", "[\"guid\",\"link\",\"category\",\"title\",\"description\",\"pubDate\",\"{http://search.yahoo.com/mrss/}thumbnail\"]", "item" },
                    { "https://www.tasnimnews.ir/fa/rss/feeds/8/0/0/0", "[\"url\"]", "[\"guid\",\"link\",\"category\",\"title\",\"description\",\"pubDate\",\"{http://search.yahoo.com/mrss/}thumbnail\"]", "item" },
                    { "https://www.yjc.ir/fa/rss/11", "[\"url\"]", "[\"title\",\"link\",\"description\",\"enclosure\",\"pubDate\"]", "item" },
                    { "https://www.yjc.ir/fa/rss/3", "[\"url\"]", "[\"title\",\"link\",\"description\",\"enclosure\",\"pubDate\"]", "item" },
                    { "https://www.yjc.ir/fa/rss/4", "[\"url\"]", "[\"title\",\"link\",\"description\",\"enclosure\",\"pubDate\"]", "item" },
                    { "https://www.yjc.ir/fa/rss/6", "[\"url\"]", "[\"title\",\"link\",\"description\",\"enclosure\",\"pubDate\"]", "item" },
                    { "https://www.yjc.ir/fa/rss/7", "[\"url\"]", "[\"title\",\"link\",\"description\",\"enclosure\",\"pubDate\"]", "item" },
                    { "https://www.yjc.ir/fa/rss/9", "[\"url\"]", "[\"title\",\"link\",\"description\",\"enclosure\",\"pubDate\"]", "item" },
                    { "https://www.yjc.ir/fa/rss/allnews", "[\"url\"]", "[\"title\",\"link\",\"description\",\"enclosure\",\"pubDate\"]", "item" }
                });

            migrationBuilder.InsertData(
                table: "RssItemFieldKeyConfigTable",
                columns: new[] { "RssFeedEfEntityId", "Argb8BitImageKey", "CategoryKey", "DescriptionKey", "IdKey", "LinkKey", "PublishDateKey", "TitleKey" },
                values: new object[,]
                {
                    { "https://www.tasnimnews.ir/fa/rss/feed/0/0/8/0/AllStories", "url", "category", "description", "guid", "link", "pubDate", "title" },
                    { "https://www.tasnimnews.ir/fa/rss/feed/0/0/8/1/TopStories", "url", "category", "description", "guid", "link", "pubDate", "title" },
                    { "https://www.tasnimnews.ir/fa/rss/feed/1127/0/7/0/MostPupolar", "url", "category", "description", "guid", "link", "pubDate", "title" },
                    { "https://www.tasnimnews.ir/fa/rss/feeds/1/0/0/0", "url", "category", "description", "guid", "link", "pubDate", "title" },
                    { "https://www.tasnimnews.ir/fa/rss/feeds/4/0/0/0", "url", "category", "description", "guid", "link", "pubDate", "title" },
                    { "https://www.tasnimnews.ir/fa/rss/feeds/8/0/0/0", "url", "category", "description", "guid", "link", "pubDate", "title" },
                    { "https://www.yjc.ir/fa/rss/11", "url", null, "description", "link", "link", "pubDate", "title" },
                    { "https://www.yjc.ir/fa/rss/3", "url", null, "description", "link", "link", "pubDate", "title" },
                    { "https://www.yjc.ir/fa/rss/4", "url", null, "description", "link", "link", "pubDate", "title" },
                    { "https://www.yjc.ir/fa/rss/6", "url", null, "description", "link", "link", "pubDate", "title" },
                    { "https://www.yjc.ir/fa/rss/7", "url", null, "description", "link", "link", "pubDate", "title" },
                    { "https://www.yjc.ir/fa/rss/9", "url", null, "description", "link", "link", "pubDate", "title" },
                    { "https://www.yjc.ir/fa/rss/allnews", "url", null, "description", "link", "link", "pubDate", "title" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RssFeedTable_RssSiteEfEntityId",
                table: "RssFeedTable",
                column: "RssSiteEfEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_RssItemTable_PublishDate",
                table: "RssItemTable",
                column: "PublishDate");

            migrationBuilder.CreateIndex(
                name: "IX_RssItemTable_RssFeedEfEntityId",
                table: "RssItemTable",
                column: "RssFeedEfEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RssFeedXmlParsingConfigTable");

            migrationBuilder.DropTable(
                name: "RssItemFieldKeyConfigTable");

            migrationBuilder.DropTable(
                name: "RssItemTable");

            migrationBuilder.DropTable(
                name: "RssFeedTable");

            migrationBuilder.DropTable(
                name: "RssSiteTable");
        }
    }
}
