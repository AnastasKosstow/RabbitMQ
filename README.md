<h3 align="center">
RabbitMQ
</h3>

****

Table of content
-----------------
* [Introduction](#introduction)


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
