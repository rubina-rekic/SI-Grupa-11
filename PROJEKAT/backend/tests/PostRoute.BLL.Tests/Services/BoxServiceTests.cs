using Moq;
using Xunit;
using PostRoute.BLL.Services;
using PostRoute.DAL.Repositories;
using PostRoute.DAL.Entities;
using PostRoute.BLL.Commands;
using PostRoute.BLL.Exceptions;

namespace PostRoute.BLL.Tests.Services;

public class BoxServiceTests
{
    private readonly Mock<IBoxRepository> _repositoryMock;
    private readonly BoxService _service;

    public BoxServiceTests()
    {
        _repositoryMock = new Mock<IBoxRepository>();
        // Pretpostavljamo da BoxService prima IBoxRepository u konstruktoru
        _service = new BoxService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenSerialNumberAlreadyExists()
    {
        // ARRANGE
        var duplicateSn = "sn-000";
        var command = new CreateBoxCommand(
            "Test Adresa", 45.0m, 45.0m, "Zidni (mali)", duplicateSn, 50, 2025);

        // Simuliramo da u bazi već postoji sandučić sa tim serijskim brojem
        _repositoryMock
            .Setup(r => r.SerialNumberExistsAsync(duplicateSn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true); // Kažemo testu: "Da, ovaj serijski broj postoji"
        // ACT & ASSERT
        // Proveravamo da li servis baca tačno SerialNumberAlreadyExistsException
        await Assert.ThrowsAsync<SerialNumberAlreadyExistsException>(() => 
            _service.CreateAsync(command, CancellationToken.None));
            
        // Proveravamo da se AddAsync NIKADA nije pozvao (jer je duplikat)
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Box>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}