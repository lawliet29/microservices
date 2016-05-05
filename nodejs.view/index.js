const bus = require('servicebus').bus({
    url: 'amqp://pcyuoarf:mm_DAba1hDupi1KnsR5l9kVsTupsBo3V@chicken.rmq.cloudamqp.com/pcyuoarf'
});

const queue = {
    ack: true
};

bus.listen('viewEvent', queue, message => {
    console.log(message);    
});