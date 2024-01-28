using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using UnityEngine;

public class RabbitMqService
{
    //public enum ROUTING_KEY
    //{
    //    SPAWN, INPUT, KILL
    //}

    IConnection connection;
    IModel channel;
    Dictionary<string, string> subjectToQueueName;

    public RabbitMqService(string[] subjects)
    {
        Debug.Log("Connecting to rabbitmq");
        connectToRabbitMq(subjects);
    }

    public void destroy()
    {
        connection.Close();
        channel.Close();
    }

    public void publish(string subject, string content)
    {
        if (channel == null)
        {
            return;
        }

        IBasicProperties props = channel.CreateBasicProperties();
        props.DeliveryMode = 2;

        channel.BasicPublish("multiplayer", subject.ToString(), props, System.Text.Encoding.UTF8.GetBytes(content));
    }

    public void subscribe(string subject, BasicDeliverEventHandler lambda)
    {

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += lambda;

        channel.BasicConsume(subjectToQueueName[subject], true, consumer);
    }

    private void connectToRabbitMq(string[] subjects)
    {
        ConnectionFactory connectionFactory = createConnectionFactory();
        subjectToQueueName = new Dictionary<string, string>();

        connection = connectionFactory.CreateConnection();
        channel = connection.CreateModel();

        channel.ExchangeDeclare("multiplayer", ExchangeType.Direct);

        foreach (var value in subjects)
        {

            var result = channel.QueueDeclare("", true, true, true, null);
            var queueName = result.QueueName;
            subjectToQueueName.Add(value, queueName);

            channel.QueueBind(queueName, "multiplayer", value.ToString());
        }
    }

    private ConnectionFactory createConnectionFactory()
    {
        ConnectionFactory factory = new ConnectionFactory();

        factory.UserName = "guest";
        factory.Password = "guest";
        factory.HostName = "localhost";

        //factory.UserName = "pedro";
        //factory.Password = "pedro";
        //factory.HostName = "10.72.119.151";
        // factory.HostName = "192.168.1.5";

        return factory;
    }
}
