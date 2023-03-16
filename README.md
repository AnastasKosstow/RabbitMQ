<h3 align="center">
RabbitMQ
</h3>

****

Table of content
-----------------
* [Introduction](#introduction)
* [Exchanges](#exchanges)
  * [Direct exchange](#direct-exchange)


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

<br><br>

Exchanges
==========================

In the AMQP, exchanges are the components responsible for routing messages to the appropriate queues based on specific routing rules or bindings.
<br>
When a producer sends a message to the exchange, the exchange receives the message and decides how to route it to the queues based on the type of exchange and the bindings that have been configured. The exchange then forwards the message to the appropriate queue(s) according to these rules.


<h3>Direct exchange</h3>
A Direct exchange in AMQP is a type of exchange that routes messages to a queue based on a direct match between the routing key of the message and the binding key of the queue.
<br>

When a message is sent to a Direct exchange, the exchange looks at the routing key of the message and compares it to the binding key of each queue that is bound to the exchange. If the routing key and the binding key match exactly, the message is routed to the corresponding queue.
<br>

For example, if a message has a routing key of "customer.orders" and there is a queue bound to the exchange with a binding key of "customer.orders", the message will be routed to that queue. However, if there is no queue bound to the exchange with a matching binding key, the message will be discarded.

















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



