const bus = require('servicebus').bus({
    url: 'amqp://dmowhoix:n95a17-CgycVl9cHsxXOCbVhX-R5iEDP@hare.rmq.cloudamqp.com/dmowhoix'
});

const queue = {
    ack: true
};

bus.listen('viewEvent', queue, message => {
    console.log(message);    
});