namespace DisputePortal.UnitTests
{
    public class DisputeTests
    {
        // Set up
        private static readonly string ReviewReason = "Security audit team is reviewing this dispute";

        private static Dispute CreateNewDispute() =>
                new(transactionId: Guid.NewGuid(),
                customerId: Guid.NewGuid(),
                category: DisputeCategory.UnauthorizedTransaction,
                reason: "Customer did not make this purchase");

        [Fact]
        public void NewDispute_StartsSubmitted_WithOneNewHistoryRow()
        {
            // Arrange + Act
            var dispute = CreateNewDispute();

            // Assert
            Assert.Equal(DisputeStatus.Submitted, dispute.Status);
            Assert.Single(dispute.StatusHistory);

            var newDisputeHistoryEntity = dispute.StatusHistory.First();
            Assert.Null(newDisputeHistoryEntity.FromStatus);  // Business rule sates: new disputes cannot have a 'from' status                             
            Assert.Equal(DisputeStatus.Submitted, newDisputeHistoryEntity.ToStatus);
        }

        [Fact]
        public void Resolve_FromUnderReview_ByAgent_Succeeds()
        {
            // Arrange
            var agentId = Guid.NewGuid();
            var resultReason = "Security audit team has reviewed successfully";
            var dispute = CreateNewDispute();

            // Act
            dispute.MoveToUnderReview(agentId, ReviewReason);
            dispute.Resolve(agentId, resultReason);

            // Assert
            Assert.Equal(3, dispute.StatusHistory.Count);

            var lastStatusUpdate = dispute.StatusHistory.Last();
            Assert.Equal(DisputeStatus.UnderReview, lastStatusUpdate.FromStatus);
            Assert.Equal(DisputeStatus.Resolved, lastStatusUpdate.ToStatus);
            Assert.Equal(agentId, lastStatusUpdate.ChangedBy);
            Assert.Equal(resultReason, lastStatusUpdate.Note);
        }

        [Fact]
        public void Reject_FromUnderReview_ByAgent_Succeeds()
        {
            // Arrange
            var dispute = CreateNewDispute();
            var agentId = Guid.NewGuid();
            var rejectReason = "transaction was done by customer's partner";

            // Act
            dispute.MoveToUnderReview(agentId);
            dispute.Reject(agentId, rejectReason);

            // Assert
            Assert.Equal(DisputeStatus.Rejected, dispute.Status);
        }

        [Fact]
        public void Resolve_WhenJustSubmitted_Throws()
        {
            // Arrange
            var dispute = CreateNewDispute();
            var agentId = Guid.NewGuid();
            var resolveReason = "quietly resolving issue caused by agent";
            // Act + Assert
            Assert.Throws<InvalidDisputeTransitionException>(() => dispute.Resolve(agentId, resolveReason));
            // Checks to see if there is any trace of the dispute alteration.
            Assert.Equal(DisputeStatus.Submitted, dispute.Status);
            Assert.NotEqual(resolveReason, dispute.Reason);
            Assert.Single(dispute.StatusHistory);
        }


        [Fact]
        public void Resolve_AfterWithdrawn_Throws()
        {
            // Arrange 
            var dispute = CreateNewDispute();
            var customerId = dispute.CustomerId;
            var agentId = Guid.NewGuid();

            // Act
            dispute.Withdraw(customerId, null);

            // Assert
            Assert.Throws<InvalidDisputeTransitionException>(() => dispute.Resolve(agentId, null));
            Assert.Equal(DisputeStatus.Withdrawn, dispute.Status);
            Assert.Equal(2, dispute.StatusHistory.Count);
        }
    }
}