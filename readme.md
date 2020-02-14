# Description
This repository contains a simple implementation of Pub/Sub in .NET Core. This code has been used as example accompaining a series of articles on my personal blog: https://www.davideguida.com/consuming-message-queues-using-net-core-background-workers-part-1-message-queues/


## The Publisher
The Publisher is implemented a simple Console application. The user will be prompted to write a text message which will be sent to a RabbitMQ fanout exchange.

## The Subscriber
The Subscriber is implemented as a .NET Core Background Worker hosted in a Web API. The Worker is starting the subscriber and listening for incoming messages. 

Messages are internally processed using a Producer/Consumer mechanism leveraging the System.Threading.Channels library.

Once a message is received, the worker will send it to a Producer which will then dispatch on a Channel. A certain number of Consumers has been registered at bootstrap. The first available Consumer will pick up the message and store it in a repository.

The Web API exposes a single GET endpoint /messages which will return the list of received messages.

For more details about Producer/Consumer, check this article: https://www.davideguida.com/how-to-implement-producer-consumer-with-system-threading-channels/
