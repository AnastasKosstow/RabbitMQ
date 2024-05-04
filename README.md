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

> [!IMPORTANT]
> AMQP (Advanced Message Queuing Protocol) is an open standard protocol for messaging middleware with following patterns:
> * Direct Exchange: Messages are routed to queues based on a matching routing key.
> * Fanout Exchange: Messages are broadcast to all queues that are bound to the exchange.
> * Topic Exchange: Messages are routed to queues based on a pattern match between the routing key of the message and the binding key of the queue.
> * Headers Exchange: Messages are routed to queues based on headers attached to the message.


### Queue

A buffer that stores messages until they are consumed by a consumer application or system. Queues have a name and can have one or more bindings to exchanges, which determine the messages that are received by the queue.

Some common properties of a queue:

 * <strong>Name</strong>: The name of the queue, which must be unique within the virtual host.
 * <strong>Durability</strong>: Whether the queue is durable or transient. A durable queue persists even if the broker is restarted, while a transient queue is deleted when the broker is restarted.
 * <strong>Exclusive</strong>: Whether the queue is exclusive to one connection or channel. Exclusive queues are only accessible to the connection that created them and are deleted when that connection is closed.
 * <strong>Auto-delete</strong>: Whether the queue is automatically deleted when it is no longer in use.


### Bindings
Bindings are relationships between a queue and an exchange that determine which messages are received by the queue. Bindings specify the routing criteria for messages to be forwarded to the queue, based on the exchange type and the message routing key.
<br>

When a message is sent to an exchange, the exchange routes the message to one or more queues based on the bindings of the queues. Each binding consists of a routing key and optional arguments that define how messages are filtered and routed to the queue.
<br>

The routing key is a message attribute that is matched against the routing criteria defined in the bindings of the exchange. The exchange uses the routing key to determine which queues should receive the message. For example, a direct exchange will forward messages to a queue if the routing key of the message matches the routing key of the queue's binding.

---

Exchanges
==========================

In the AMQP, exchanges are the components responsible for routing messages to the appropriate queues based on specific routing rules or bindings.
<br>
When a producer sends a message to the exchange, the exchange receives the message and decides how to route it to the queues based on the type of exchange and the bindings that have been configured. The exchange then forwards the message to the appropriate queue(s) according to these rules.
<br>


### Direct exchange
A Direct exchange in AMQP is a type of exchange that routes messages to a queue based on a direct match between the routing key of the message and the binding key of the queue.
<br>

When a message is sent to a Direct exchange, the exchange looks at the routing key of the message and compares it to the binding key of each queue that is bound to the exchange. If the routing key and the binding key match exactly, the message is routed to the corresponding queue.
<br>

For example, if a message has a routing key of "customer.orders" and there is a queue bound to the exchange with a binding key of "customer.orders", the message will be routed to that queue. However, if there is no queue bound to the exchange with a matching binding key, the message will be discarded.

![](https://github.com/AnastasKosstow/RabbitMQ/blob/main/assets/direct_exchange.png)
<br><br>


### Fanout exchange

A Fanout exchange in AMQP is a type of exchange that routes messages to all queues that are bound to it.
<br>

When a message is sent to a Fanout exchange, the exchange simply forwards a copy of the message to all the queues that are bound to it, regardless of the routing key of the message.
<br>

For example, if a Fanout exchange is bound to three queues, and a message is sent to the exchange, the message will be delivered to all three queues.
<br>

![](https://github.com/AnastasKosstow/RabbitMQ/blob/main/assets/fanout_exchange.png)
<br><br>


### Topic exchange

A Topic exchange in AMQP is a type of exchange that routes messages to one or more queues based on matching the routing key of the message against one or more topic bindings.
<br>

Topic bindings are defined using a pattern, which is a string that consists of one or more words, separated by dots. A routing key is also a string that can consist of one or more words, separated by dots. When a message is sent to a Topic exchange, the exchange compares the routing key of the message to the patterns of the topic bindings of all the queues that are bound to it. If the routing key matches the pattern of a binding, the message is routed to the corresponding queue.
<br>

For example, if a message has a routing key of "customer.orders.new" and there are two queues bound to the Topic exchange, one with a binding key of "customer.orders.#" and another with a binding key of "customer.#.new", the message will be delivered to both queues because it matches the patterns of both bindings.
<br>

![](https://github.com/AnastasKosstow/RabbitMQ/blob/main/assets/topic_exchange.png)
<br><br>

### Header exchange

A Headers exchange in AMQP is a type of exchange that routes messages based on header values instead of the routing key.
<br>

When a message is sent to a Headers exchange, the exchange looks at the headers of the message and compares them to the headers of the bindings of all the queues that are bound to it. If the header values of the message match the header values of a binding, the message is routed to the corresponding queue.
<br>

For example, if a message has a header of "color=blue" and there is a queue bound to the Headers exchange with a binding that specifies "x-match=all" and "color=blue", the message will be routed to that queue. However, if the message header does not match any of the bindings, the message will be discarded.
<br>

![](https://github.com/AnastasKosstow/RabbitMQ/blob/main/assets/headers_exchange.png)
<br>

---

## Producer 
A producer is responsible for sending messages to exchanges. <br>
The `RabbitMqPublisherWithConnection` class implements a producer that can publish messages to a RabbitMQ.























