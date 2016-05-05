const EventStore = require('event-store-client');
const connection = new EventStore.Connection({});

connection.subscribeToStream('viewEvent', true, e => {
    console.log('EventType: ' + e.data.EventType + ', EntityId: ' + e.data.EntityId);
});