using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;

public class ProductsApiTests : IDisposable
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;

    public ProductsApiTests()
    {
        _factory = new ApiFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededProducts()
    {
        var response = await _client.GetAsync("/api/Products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content
                                .ReadFromJsonAsync<List<ResponseProductDto>>();
        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task GetById_NotFound_Returns404()
    {
        var response = await _client.GetAsync("/api/Products/999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsSeededProduct()
    {
        var response = await _client.GetAsync("/api/Products/1");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var product = await response.Content
                                .ReadFromJsonAsync<ResponseProductDto>();
        Assert.NotNull(product);
        Assert.Equal(1, product.Id);
        Assert.Equal("ปากกา", product.Name);
        Assert.Equal(10m, product.Price);
    }

    [Fact]
    public async Task GetSearch_MatchingName_ReturnsMatchingProducts()
    {
        var response = await _client.GetAsync("/api/Products/search?name=ปากกา");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content
                                .ReadFromJsonAsync<List<ResponseProductDto>>();
        Assert.NotNull(products);
        Assert.Single(products);
        Assert.Equal("ปากกา", products[0].Name);
    }

    [Theory]
    [InlineData("ปากกา", 1)]
    [InlineData("ยางลบ", 1)]
    [InlineData("zzz", 0)]
    public async Task GetSearch_MatchingMultiName_ReturnsMatchingProducts(string name, int expectedCount)
    {
        var response = await _client.GetAsync($"/api/Products/search?name={Uri.EscapeDataString(name)}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content
                                .ReadFromJsonAsync<List<ResponseProductDto>>();
        Assert.NotNull(products);
        Assert.Equal(expectedCount, products.Count);
    }

    [Theory]
    [InlineData("ป")]
    [InlineData("ปาก")]
    [InlineData("กา")]
    public async Task GetSearch_MatchingHalfName(string name)
    {
        var response = await _client.GetAsync($"/api/Products/search?name={Uri.EscapeDataString(name)}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content
                                .ReadFromJsonAsync<List<ResponseProductDto>>();
        Assert.NotNull(products);
        Assert.Single(products);
        Assert.Equal("ปากกา", products[0].Name);
    }

    [Theory]
    [InlineData(10, 2)]
    [InlineData(45, 1)]
    [InlineData(46, 0)]
    public async Task GetExpensive_MatchingMinPrice_ReturnsMatchingProducts(decimal minPrice, int expectedCount)
    {
        var response = await _client.GetAsync($"/api/Products/expensive?minPrice={minPrice.ToString(CultureInfo.InvariantCulture)}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content
                                .ReadFromJsonAsync<List<ResponseProductDto>>();
        Assert.NotNull(products);
        Assert.Equal(expectedCount, products.Count);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(1000001)]
    public async Task Create_InvalidPrice_Returns400(decimal price)
    {
        var dto = new CreateProductDto {Name = "สมุด", Price = price};

        var response = await _client.PostAsJsonAsync("/api/Products", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidName_Returns400()
    {
        var dto = new CreateProductDto {Name = "", Price = 10};
        
        var response = await _client.PostAsJsonAsync("/api/Products", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_Succeed_Returns201()
    {
        var dto = new CreateProductDto {Name = "สมุด", Price = 10};

        var response = await _client.PostAsJsonAsync("/api/Products", dto);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var product = await response.Content.ReadFromJsonAsync<ResponseProductDto>();
        Assert.NotNull(product);
        Assert.Equal("สมุด", product.Name);
    }

    [Fact]
    public async Task Update_NonExistingId_Returns404()
    {
        var dto = new UpdateProductDto {Name = "สมุด", Price = 10};

        var response = await _client.PutAsJsonAsync("/api/Products/999", dto);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidData_Returns200AndUpdatesProduct()
    {
        var dto = new UpdateProductDto {Name = "ดินสอ", Price = 25};

        var response = await _client.PutAsJsonAsync("/api/Products/1", dto);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await response.Content.ReadFromJsonAsync<ResponseProductDto>();
        Assert.NotNull(updated);
        Assert.Equal(1, updated.Id);
        Assert.Equal("ดินสอ", updated.Name);
        Assert.Equal(25m, updated.Price);
    }

    [Fact]
    public async Task Delete_NonExistingId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/Products/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingId_Returns204ThenGetReturns404()
    {
        var response = await _client.DeleteAsync("/api/Products/1");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var get = await _client.GetAsync("/api/Products/1");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }


}
