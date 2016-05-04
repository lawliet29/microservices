using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MS.EntityAggregate;
using MS.EntityView;
using ViewEvent = MS.EntityAggregate.Dtos.ViewEvent;
using ViewEventType = MS.EntityAggregate.Dtos.ViewEventType;

namespace MS.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void Test()
        {
            // arrange
            var aggregateMock = new Mock<IEntityAggregate>();
            var aggregate = aggregateMock.Object;
            var splitItem = new SplitItem(new ExecuteCommand(new SaveItem(aggregate), new DeleteItem(aggregate)));
            const int createId = 5;
            const int deleteId = 6;

            // act
            splitItem.Split(new Command[]
            {
                new CreateCommand { EntityId = createId }, 
                new DeleteCommand { EntityId = deleteId }
            });

            // assert
            aggregateMock.Verify(a => a.ProcessCommand(
                It.Is<EntityCommand>(c => c.Type == EntityCommandType.Save && c.Entity.EntityId == createId)));

            aggregateMock.Verify(a => a.ProcessCommand(
                It.Is<EntityCommand>(c => c.Type == EntityCommandType.Delete && c.Entity.EntityId == deleteId)));

        }

        [TestMethod]
        public void Test2()
        {
            // arrange
            var viewMock = new Mock<IEntityView>();
            var view = viewMock.Object;
            MS.EntityAggregate.EntityView.Instance = view;
            var connection = new EntityAggregateServer().Connect();
            var aggregate = new RestApiEntityAggregateClient();
            var splitItem = new SplitItem(new ExecuteCommand(new SaveItem(aggregate), new DeleteItem(aggregate)));
            const int createId = 5;
            const int deleteId = 6;

            // act
            splitItem.Split(new Command[]
            {
                new CreateCommand { EntityId = createId },
                new DeleteCommand { EntityId = deleteId }
            });

            // assert
            viewMock.Verify(m => m.HandleEvent(It.Is<ViewEvent>(e => e.EntityId == createId && e.EventType == ViewEventType.EntityCreated)));
            viewMock.Verify(m => m.HandleEvent(It.Is<ViewEvent>(e => e.EntityId == deleteId && e.EventType == ViewEventType.EntityDeleted)));
            connection.Dispose();
        }

        [TestMethod]
        public void Test3()
        {
            // arrange
            var aggregateConnection = new EntityAggregateServer().Connect();
            var viewConnection = new EntityViewServer().Connect();
            var aggregate = new RestApiEntityAggregateClient();
            var splitItem = new SplitItem(new ExecuteCommand(new SaveItem(aggregate), new DeleteItem(aggregate)));
            const int createId = 5;
            const int deleteId = 6;

            // act
            splitItem.Split(new Command[]
            {
                new CreateCommand { EntityId = createId },
                new DeleteCommand { EntityId = deleteId }
            });

            // assert
            CollectionAssert.AreEquivalent(new [] { createId }, EntityView.EntityView.Instance.State.ToArray());
            aggregateConnection.Dispose();
            viewConnection.Dispose();
        }
    }
}
