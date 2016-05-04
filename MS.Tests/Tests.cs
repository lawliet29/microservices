using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MS.EntityAggregate;
using MS.EntityView;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using CollectionAssert = Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert;
using ViewEvent = MS.EntityAggregate.Dtos.ViewEvent;
using ViewEventType = MS.EntityAggregate.Dtos.ViewEventType;

namespace MS.Tests
{
    [TestFixture]
    public class Tests
    {
        
        [Test]
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

        [Test]
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
        

        [Test]
        public void Test3()
        {
            // arrange
            var subscriber = new EntityViewQueueListener().Subscribe();
            var aggregate = new EntityAggregate.EntityAggregate(new EntityViewRabbitClient());

            const int createId = 5;
            const int deleteId = 6;

            // act
            aggregate.ProcessCommand(new EntityAggregate.Dtos.EntityCommand
            {
                Entity = new EntityAggregate.Dtos.Entity { EntityId = createId }, 
                Type = EntityAggregate.Dtos.EntityCommandType.Save
            });
            aggregate.ProcessCommand(new EntityAggregate.Dtos.EntityCommand
            {
                Entity = new EntityAggregate.Dtos.Entity { EntityId = deleteId },
                Type = EntityAggregate.Dtos.EntityCommandType.Delete
            });

            Thread.Sleep(10000);

            // assert
            CollectionAssert.AreEquivalent(new [] { createId }, EntityView.EntityView.Instance.State.ToArray());
            subscriber.Dispose();
        }

        [Test]
        public void Test4()
        {
            const string queueName = "testQueue";
            var rabbitClient = new EntityViewRabbitClient(queueName);

            var entityId = 5;
            var eventType = ViewEventType.EntityCreated;

            rabbitClient.HandleEvent(new ViewEvent
            {
                EntityId = entityId,
                EventType = eventType
            });

            var tcs = new TaskCompletionSource<JObject>();

            var subscription = EntityViewQueueListener.Bus.ConsumeJson(queueName, json =>
            {
                tcs.SetResult(json);
            });

            var result = tcs.Task.Result;

            Assert.That(result["EntityId"].Value<int>(), Is.EqualTo(entityId));
            Assert.That(result["EventType"].Value<int>(), Is.EqualTo((int)eventType));
        }


    }
}
