using Moq;
using Verum.Application.DTOs.Personal;
using Verum.Application.Interfaces;
using Verum.Application.Interfaces.Repositories;
using Verum.Application.Services;
using Verum.Domain.Entities;

namespace Verum.Tests.Services;

public class CommitmentServiceTests
{
    private readonly Mock<ICommitmentRepository> _commitmentRepository = new();
    private readonly Mock<IExpenseService> _expenseService = new();
    private readonly CommitmentService _sut;
    private readonly Guid _accountId = Guid.NewGuid();

    public CommitmentServiceTests()
    {
        _sut = new CommitmentService(_commitmentRepository.Object, _expenseService.Object);
    }

    [Fact]
    public async Task MarkPaidAsync_AlreadyPaid_RejectsAndDoesNotChargeAgain()
    {
        var commitmentId = Guid.NewGuid();
        _commitmentRepository.Setup(r => r.GetAllForCurrentPeriodAsync()).ReturnsAsync(new List<Commitment>
        {
            new() { Id = commitmentId, Name = "Netflix", Amount = 299m, IsPaid = true }
        });

        var result = await _sut.MarkPaidAsync(commitmentId, _accountId);

        Assert.False(result.Success);
        _expenseService.Verify(s => s.RegisterExpenseAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task MarkPaidAsync_DoesNotMarkPaid_WhenExpenseRegistrationFails()
    {
        var commitmentId = Guid.NewGuid();
        _commitmentRepository.Setup(r => r.GetAllForCurrentPeriodAsync()).ReturnsAsync(new List<Commitment>
        {
            new() { Id = commitmentId, Name = "Internet", Amount = 600m, IsPaid = false }
        });
        _expenseService.Setup(s => s.RegisterExpenseAsync(_accountId, It.IsAny<string>(), 600m))
            .ReturnsAsync(new RegisterExpenseResult(false, "sin saldo", 0, 0));

        var result = await _sut.MarkPaidAsync(commitmentId, _accountId);

        Assert.False(result.Success);
        _commitmentRepository.Verify(r => r.MarkPaidAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task MarkPaidAsync_MarksPaid_WhenExpenseRegistrationSucceeds()
    {
        var commitmentId = Guid.NewGuid();
        _commitmentRepository.Setup(r => r.GetAllForCurrentPeriodAsync()).ReturnsAsync(new List<Commitment>
        {
            new() { Id = commitmentId, Name = "Internet", Amount = 600m, IsPaid = false }
        });
        _expenseService.Setup(s => s.RegisterExpenseAsync(_accountId, It.IsAny<string>(), 600m))
            .ReturnsAsync(new RegisterExpenseResult(true, null, 1000m, 400m));

        var result = await _sut.MarkPaidAsync(commitmentId, _accountId);

        Assert.True(result.Success);
        _commitmentRepository.Verify(r => r.MarkPaidAsync(commitmentId), Times.Once);
    }
}
