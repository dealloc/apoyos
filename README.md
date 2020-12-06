# Apoyos

## How to use the servicebus

Add the following packages to your csproj:

```xml
<PackageReference Include="Apoyos.Servicebus" Version="1.0.0-rc" />
<PackageReference Include="Apoyos.Servicebus.RabbitMQ" Version="1.0.0-rc" />
```

Add the servicebus to your container:
```csharp
services.AddRabbitMQ(Configuration.GetSection("Servicebus"));
```

For all events you want to publish, add:
```csharp
services.AddDomainEvent<MyDomainEvent>("my-domain-event");
```
And for all event handlers you want to listen for events, add:
```csharp
services.AddDomainEventListener<MyDomainEvent, MyDomainEventHandler>("my-domain-event");
```
Your configuration should look something like this:
```json
{
  "Servicebus": {
    "ServiceName": "MyApplicationName",
    "RabbitMQ": {
      "Hostname": "RabbitMQHostname",
      "Username": "guest",
      "Password": "guest"
    }
  }
}
```

## Using a custom serializer
By default Apoyos uses the JsonDomainEventSerializer in the `Apoyos.Servicebus` package.

Simply implement the `IDomainEventSerializer` interface and register it in the container after `AddRabbitMQ`.