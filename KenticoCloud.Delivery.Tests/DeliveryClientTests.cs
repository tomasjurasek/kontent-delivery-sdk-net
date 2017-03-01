﻿using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KenticoCloud.Delivery.Tests
{
    [TestFixture]
    public class DeliveryClientTests
    {
        private const string PROJECT_ID = "975bf280-fd91-488c-994c-2f04416e5ee3";

        [Test]
        public void GetItemAsync()
        {
            var client = new DeliveryClient(PROJECT_ID);
            var beveragesItem = Task.Run(() => client.GetItemAsync("coffee_beverages_explained")).Result.Item;
            var barraItem = Task.Run(() => client.GetItemAsync("brazil_natural_barra_grande")).Result.Item;
            var roastsItem = Task.Run(() => client.GetItemAsync("on_roasts")).Result.Item;
            Assert.AreEqual("article", beveragesItem.System.Type);
            Assert.GreaterOrEqual(beveragesItem.System.SitemapLocation.Count, 1);
            Assert.GreaterOrEqual(roastsItem.GetModularContent("related_articles").Count(), 1);
            Assert.AreEqual(beveragesItem.Elements.title.value.ToString(), beveragesItem.GetString("title"));
            Assert.AreEqual(beveragesItem.Elements.body_copy.value.ToString(), beveragesItem.GetString("body_copy"));
            Assert.AreEqual(DateTime.Parse(beveragesItem.Elements.post_date.value.ToString()), beveragesItem.GetDateTime("post_date"));
            Assert.AreEqual(beveragesItem.Elements.teaser_image.value.Count, beveragesItem.GetAssets("teaser_image").Count());
            Assert.AreEqual(beveragesItem.Elements.personas.value.Count, beveragesItem.GetTaxonomyTerms("personas").Count());
            Assert.AreEqual(decimal.Parse(barraItem.Elements.price.value.ToString()), barraItem.GetNumber("price"));
            Assert.AreEqual(barraItem.Elements.processing.value.Count, barraItem.GetOptions("processing").Count());
        }

        [Test]
        public void GetItemAsync_NotFound()
        {
            var client = new DeliveryClient(PROJECT_ID);
            AsyncTestDelegate action = async () => await client.GetItemAsync("unscintillating_hemerocallidaceae_des_iroquois");

            Assert.ThrowsAsync<DeliveryException>(action);
        }

        [Test]
        public void GetItemsAsync()
        {
            var client = new DeliveryClient(PROJECT_ID);
            var response = Task.Run(() => client.GetItemsAsync(new EqualsFilter("system.type", "cafe"))).Result;

            Assert.GreaterOrEqual(response.Items.Count, 1);
        }

        [Test]
        public void GetTypeAsync()
        {
            var client = new DeliveryClient(PROJECT_ID);
            var articleType = Task.Run(() => client.GetTypeAsync("article")).Result;
            var coffeeType = Task.Run(() => client.GetTypeAsync("coffee")).Result;
            var taxonomyElement = articleType.Elements["personas"] as TaxonomyContentElement;
            var multipleChoiceElement = coffeeType.Elements["processing"] as MultipleChoiceContentElement;

            Assert.AreEqual("article", articleType.System.Codename);
            Assert.AreEqual("text", articleType.Elements["title"].Type);
            Assert.AreEqual("rich_text", articleType.Elements["body_copy"].Type);
            Assert.AreEqual("date_time", articleType.Elements["post_date"].Type);
            Assert.AreEqual("asset", articleType.Elements["teaser_image"].Type);
            Assert.AreEqual("modular_content", articleType.Elements["related_articles"].Type);
            Assert.AreEqual("taxonomy", articleType.Elements["personas"].Type);
            Assert.AreEqual("number", coffeeType.Elements["price"].Type);
            Assert.AreEqual("multiple_choice", coffeeType.Elements["processing"].Type);

            Assert.AreEqual("personas", taxonomyElement.TaxonomyGroup);
            Assert.GreaterOrEqual(multipleChoiceElement.Options.Count, 1);
        }

        [Test]
        public void GetTypeAsync_NotFound()
        {
            var client = new DeliveryClient(PROJECT_ID);
            AsyncTestDelegate action = async () => await client.GetTypeAsync("unequestrian_nonadjournment_sur_achoerodus");

            Assert.ThrowsAsync<DeliveryException>(action);
        }

        [Test]
        public void GetTypesAsync()
        {
            var client = new DeliveryClient(PROJECT_ID);
            var response = Task.Run(() => client.GetTypesAsync(new SkipParameter(1))).Result;

            Assert.GreaterOrEqual(response.Types.Count, 1);
        }

        [Test]
        public void GetContentElementAsync()
        {
            var client = new DeliveryClient(PROJECT_ID);
            var element = Task.Run(() => client.GetContentElementAsync("article", "title")).Result;
            var taxonomyElement = Task.Run(() => client.GetContentElementAsync("article", "personas")).Result as TaxonomyContentElement;
            var multipleChoiceElement = Task.Run(() => client.GetContentElementAsync("coffee", "processing")).Result as MultipleChoiceContentElement;

            Assert.AreEqual("title", element.Codename);
            Assert.AreEqual("personas", taxonomyElement.TaxonomyGroup);
            Assert.GreaterOrEqual(multipleChoiceElement.Options.Count, 1);
        }

        [Test]
        public void GetContentElementsAsync_NotFound()
        {
            var client = new DeliveryClient(PROJECT_ID);
            AsyncTestDelegate action = async () => await client.GetContentElementAsync("anticommunistical_preventure_sur_helxine", "unlacerated_topognosis_sur_nonvigilantness");

            Assert.ThrowsAsync<DeliveryException>(action);
        }

        [Test]
        public void QueryParameters()
        {
            var client = new DeliveryClient(PROJECT_ID);
            var parameters = new IQueryParameter[]
            {
                new AllFilter("elements.personas", "barista", "coffee", "blogger"),
                new AnyFilter("elements.personas", "barista", "coffee", "blogger"),
                new ContainsFilter("system.sitemap_locations", "cafes"),
                new EqualsFilter("elements.product_name", "Hario V60"),
                new GreaterThanFilter("elements.price", "1000"),
                new GreaterThanOrEqualFilter("elements.price", "50"),
                new InFilter("system.type", "cafe", "coffee"),
                new LessThanFilter("elements.price", "10"),
                new LessThanOrEqualFilter("elements.price", "4"),
                new RangeFilter("elements.country", "Guatemala", "Nicaragua"),
                new DepthParameter(2),
                new ElementsParameter("price", "product_name"),
                new LimitParameter(10),
                new OrderParameter("elements.price", SortOrder.Descending),
                new SkipParameter(2)
            };
            var response = Task.Run(() => client.GetItemsAsync(parameters)).Result;

            Assert.AreEqual(0, response.Items.Count);
        }

        [Test]
        public void GetStronglyTypedResponse()
        {
            const string SANDBOX_PROJECT_ID = "e1167a11-75af-4a08-ad84-0582b463b010";

            // Arrange
            var client = new DeliveryClient(SANDBOX_PROJECT_ID);

            // Act
            CompleteContentItemModel item = client.GetItemAsync<CompleteContentItemModel>("complete_content_item").Result.Item;

            // Assert
            Assert.AreEqual("Text field value", item.TextField);

            Assert.AreEqual("<p>Rich text field value</p>", item.RichTextField);

            Assert.AreEqual(99, item.NumberField);

            Assert.AreEqual(1, item.MultipleChoiceFieldAsRadioButtons.Count());
            Assert.AreEqual("Radio button 1", item.MultipleChoiceFieldAsRadioButtons.First().Name);

            Assert.AreEqual(2, item.MultipleChoiceFieldAsCheckboxes.Count());
            Assert.AreEqual("Checkbox 1", item.MultipleChoiceFieldAsCheckboxes.First().Name);
            Assert.AreEqual("Checkbox 2", item.MultipleChoiceFieldAsCheckboxes.ElementAt(1).Name);

            Assert.AreEqual(new DateTime(2017, 2, 23), item.DateTimeField);

            Assert.AreEqual(1, item.AssetField.Count());
            Assert.AreEqual("Fire.jpg", item.AssetField.First().Name);
            Assert.AreEqual(129170, item.AssetField.First().Size);
            Assert.AreEqual("https://assets.kenticocloud.com:443/e1167a11-75af-4a08-ad84-0582b463b010/64096741-b658-46ee-b148-b287fe03ea16/Fire.jpg", item.AssetField.First().Url);

            Assert.AreEqual(1, item.ModularContentField.Count());
            Assert.AreEqual("Homepage", item.ModularContentField.First().System.Name);

            Assert.AreEqual(2, item.CompleteTypeTaxonomy.Count());
            Assert.AreEqual("Option 1", item.CompleteTypeTaxonomy.First().Name);
        }
    }
}
