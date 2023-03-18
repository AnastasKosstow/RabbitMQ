<h3 align="center">
RabbitMQ
</h3>

****

Table of content
-----------------
* [Introduction](#introduction)
* [Exchanges](#exchanges)
  * [Direct exchange](#direct-exchange)
  * [Fanout exchange](#fanout-exchange)
  * [Topic exchange](#topic-exchange)
  * [Header exchange](#header-exchange)
* [Configuration](#configuration)


Introduction
==========================

RabbitMQ is built on the Advanced Message Queuing Protocol (AMQP), an open standard for messaging middleware. It is written in the Erlang programming language.
<br><br>
RabbitMQ is a centralized message broker, meaning that it provides a single point of control for managing and routing messages between applications or services. 
<br>
In a centralized architecture, there is typically a single RabbitMQ server (or a cluster of servers) that acts as the central hub for message exchange.
<br><br>
In a RabbitMQ centralized architecture, applications or services that need to exchange messages connect to the central RabbitMQ server(s) and declare queues and exchanges. The server(s) then route messages between these queues and exchanges based on their routing keys and other properties.
<br>

> AMQP (Advanced Message Queuing Protocol) is an open standard protocol for messaging middleware with following patterns:
> * Direct Exchange: Messages are routed to queues based on a matching routing key.
> * Fanout Exchange: Messages are broadcast to all queues that are bound to the exchange.
> * Topic Exchange: Messages are routed to queues based on a pattern match between the routing key of the message and the binding key of the queue.
> * Headers Exchange: Messages are routed to queues based on headers attached to the message.


<h3>Queue</h3>

A buffer that stores messages until they are consumed by a consumer application or system. Queues have a name and can have one or more bindings to exchanges, which determine the messages that are received by the queue.

Some common properties of a queue:

 * <strong>Name</strong>: The name of the queue, which must be unique within the virtual host.
 * <strong>Durability</strong>: Whether the queue is durable or transient. A durable queue persists even if the broker is restarted, while a transient queue is deleted when the broker is restarted.
 * <strong>Exclusive</strong>: Whether the queue is exclusive to one connection or channel. Exclusive queues are only accessible to the connection that created them and are deleted when that connection is closed.
 * <strong>Auto-delete</strong>: Whether the queue is automatically deleted when it is no longer in use.


<h3>Bindings</h3>

Bindings are relationships between a queue and an exchange that determine which messages are received by the queue. Bindings specify the routing criteria for messages to be forwarded to the queue, based on the exchange type and the message routing key.
<br>

When a message is sent to an exchange, the exchange routes the message to one or more queues based on the bindings of the queues. Each binding consists of a routing key and optional arguments that define how messages are filtered and routed to the queue.
<br>

The routing key is a message attribute that is matched against the routing criteria defined in the bindings of the exchange. The exchange uses the routing key to determine which queues should receive the message. For example, a direct exchange will forward messages to a queue if the routing key of the message matches the routing key of the queue's binding.


<br><br>

Exchanges
==========================

In the AMQP, exchanges are the components responsible for routing messages to the appropriate queues based on specific routing rules or bindings.
<br>
When a producer sends a message to the exchange, the exchange receives the message and decides how to route it to the queues based on the type of exchange and the bindings that have been configured. The exchange then forwards the message to the appropriate queue(s) according to these rules.
<br>


<h3>Direct exchange</h3>
A Direct exchange in AMQP is a type of exchange that routes messages to a queue based on a direct match between the routing key of the message and the binding key of the queue.
<br>

When a message is sent to a Direct exchange, the exchange looks at the routing key of the message and compares it to the binding key of each queue that is bound to the exchange. If the routing key and the binding key match exactly, the message is routed to the corresponding queue.
<br>

For example, if a message has a routing key of "customer.orders" and there is a queue bound to the exchange with a binding key of "customer.orders", the message will be routed to that queue. However, if there is no queue bound to the exchange with a matching binding key, the message will be discarded.

![](https://github.com/AnastasKosstow/RabbitMQ/blob/main/assets/direct_exchange.png)
<br><br>


<h3>Fanout exchange</h3>

A Fanout exchange in AMQP is a type of exchange that routes messages to all queues that are bound to it.
<br>

When a message is sent to a Fanout exchange, the exchange simply forwards a copy of the message to all the queues that are bound to it, regardless of the routing key of the message.
<br>

For example, if a Fanout exchange is bound to three queues, and a message is sent to the exchange, the message will be delivered to all three queues.
<br>

![](https://github.com/AnastasKosstow/RabbitMQ/blob/main/assets/fanout_exchange.png)
<br><br>


<h3>Topic exchange</h3>

A Topic exchange in AMQP is a type of exchange that routes messages to one or more queues based on matching the routing key of the message against one or more topic bindings.
<br>

Topic bindings are defined using a pattern, which is a string that consists of one or more words, separated by dots. A routing key is also a string that can consist of one or more words, separated by dots. When a message is sent to a Topic exchange, the exchange compares the routing key of the message to the patterns of the topic bindings of all the queues that are bound to it. If the routing key matches the pattern of a binding, the message is routed to the corresponding queue.
<br>

For example, if a message has a routing key of "customer.orders.new" and there are two queues bound to the Topic exchange, one with a binding key of "customer.orders.#" and another with a binding key of "customer.#.new", the message will be delivered to both queues because it matches the patterns of both bindings.
<br>

![](https://github.com/AnastasKosstow/RabbitMQ/blob/main/assets/topic_exchange.png)
<br><br>

<h3>Header exchange</h3>

A Headers exchange in AMQP is a type of exchange that routes messages based on header values instead of the routing key.
<br>

When a message is sent to a Headers exchange, the exchange looks at the headers of the message and compares them to the headers of the bindings of all the queues that are bound to it. If the header values of the message match the header values of a binding, the message is routed to the corresponding queue.
<br>

For example, if a message has a header of "color=blue" and there is a queue bound to the Headers exchange with a binding that specifies "x-match=all" and "color=blue", the message will be routed to that queue. However, if the message header does not match any of the bindings, the message will be discarded.
<br>

![](https://github.com/AnastasKosstow/RabbitMQ/blob/main/assets/headers_exchange.png)
<br><br>


Configuration
==========================

The first step is to create a new instance of the `ConnectionFactory` class and configure it with the appropriate RabbitMQ connection settings, such as port number, virtual host name, username, and password.
<br>

After configuring the connection factory, you then create a connection to the RabbitMQ server using the `CreateConnection` method and passing in a list of hostnames to connect to. You then register the connection and a channel to the dependency injection container using the `services.AddSingleton` method, which allows you to inject these dependencies into other classes throughout your application.

<br>

> `IConnection` represents a connection to a RabbitMQ server. It allows you to create and manage channels.

> `IModel` represents a channel to a RabbitMQ server. It allows you to perform operations such as declaring exchanges, queues, and bindings, as well as publishing and consuming messages.
<br>


```C#
var connectionFactory = new ConnectionFactory
{
    Port = rabbitMqOptions.Port,
    VirtualHost = rabbitMqOptions.VirtualHost,
    UserName = rabbitMqOptions.Username,
    Password = rabbitMqOptions.Password,
    DispatchConsumersAsync = true,
};

services.AddSingleton(connectionFactory.CreateConnection(rabbitMqOptions.HostNames.ToList()));
var connection = connectionFactory.CreateConnection();

services.AddSingleton<IConnection>(connection);
services.AddSingleton<IModel>(connection.CreateModel());
```

<h3>Producer</h3>





<h3>Consumer</h3>
















<br><br><br><br><br>









As we know, Nothing is perfect!
==========================

<br>

<h2>Pros</h2>

 * <h4>Modules Decoupling</h4>
By adding message broker as a way communication your modules became independent from each other one module declare the message/event and any other module can consume this and perform the action and any part of this cycle can be replaceable any time

 * <h4>Reliability</h4>
RMQ provides acknowledgment facility which that any module consume message should acknowledge that it processed successfully if not the message will return to the queue again to another processing.
Note: you have to pay attention to this as RMQ will try to push failed messages again and again till it processed , so you have you have to add try/catch block to consumer code or add dead-letter extension so that any failed message could be delivered to another queue for investigation and another way of handle

 * <h4>Durable</h4>
messages can be stored on disk for avoid any data loss during any server shutdown or failure

 * <h4>Scalability</h4>
You can scale your message broker by adding resources or server instances

 * <h4>Multi-thread consuming</h4>
In case of hot you can multi process messages by enable multi thread consuming feature

<br>

<h2>Cons</h2>

 * <h4>A lot of components</h4>
There is a lot components regarding to this protocol which makes it somehow complex to understand or use.

* <h4>Push Model</h4>
RMQ server pushes messages to consumers, Whenever there is a consumer send the message to it, in case of huge amount of message consumer could crash if it can not handle this number of messages.
