using EasyTgBot.Abstract;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Stateless;
using Telegram.Bot.Types;

namespace EasyTgBot.Tests.ContextAddExtension;

public class Tests
{
    private IServiceCollection collection;

    [SetUp]
    public void Setup()
    {
        collection = new ServiceCollection();
        collection.AddSingleton<IServiceRegistryFlow, ServiceRegistryFlow>();
    }

    [Test]
    public void CorrectWork_IF_UseAddHandlerAndAddSubHandle()
    {
        var serviceRegistry = collection.BuildServiceProvider().GetService<IServiceRegistryFlow>();
        collection.AddContext<TestUserFlow>("test",
            x => x.AddSwitch<FakeSwitch>(
                    (x => x.AddHandler<FakeHandler>(), "whoap"),
                    (x => x.AddHandler<FakeHandler>(), "lol"))
                .AddHandler<FakeHandler>(),
            serviceRegistry);

        var stateMachine = new StateMachine<TestUserFlow, Trigger>(TestUserFlow.Authorization);
        serviceRegistry.Wraps(stateMachine);

        var enums = Enum.GetValues<TestUserFlow>();
        var states = stateMachine.GetInfo().States.ToArray();
        states.Length.Should().Be(enums.Length);
        for (var i = 0; i < enums.Length; i++)
        {
            states[i].UnderlyingState.Should().Be(enums[i]);
        }


        var userGoToSubTask =
            new StateMachine<TestUserFlow, Trigger>.TriggerWithParameters<string>(Trigger.UserGoToSubTask);


        stateMachine.Fire(userGoToSubTask, TestUserFlow.AddSecondName.ToString());
        stateMachine.State.Should().Be(TestUserFlow.AddSecondName);


        stateMachine = new StateMachine<TestUserFlow, Trigger>(TestUserFlow.Authorization);
        serviceRegistry.Wraps(stateMachine);
        stateMachine.Fire(userGoToSubTask, TestUserFlow.AddOld.ToString());
        stateMachine.State.Should().Be(TestUserFlow.AddOld);

        stateMachine = new StateMachine<TestUserFlow, Trigger>(TestUserFlow.Authorization);
        serviceRegistry.Wraps(stateMachine);
        stateMachine.Fire(Trigger.UserWantToContinue);
        stateMachine.State.Should().Be(TestUserFlow.AddName);
    }

    [Test]
    public void CorrectWork_IF_UseAddHandler()
    {
        var serviceRegistry = collection.BuildServiceProvider().GetService<IServiceRegistryFlow>();
        collection.AddContext<TestUserFlow>("test",
            x => x.AddHandler<FakeHandler>()
                .AddHandler<FakeHandler2>()
                .AddHandler<FakeHandler2>()
                .AddHandler<FakeHandler>(),
            serviceRegistry);

        var stateMachine = new StateMachine<TestUserFlow, Trigger>(TestUserFlow.Authorization);
        serviceRegistry.Wraps(stateMachine);

        var enums = Enum.GetValues<TestUserFlow>();
        var states = stateMachine.GetInfo().States.ToArray();
        enums.Length.Should().Be(states.Length);
        for (var i = 0; i < enums.Length; i++)
        {
            states[i].UnderlyingState.Should().Be(enums[i]);
        }


        stateMachine.State.Should().Be(TestUserFlow.Authorization);
        stateMachine.Fire(Trigger.UserWantToContinue);

        stateMachine.State.Should().Be(TestUserFlow.AddSecondName);
        stateMachine.Fire(Trigger.UserWantToContinue);

        stateMachine.State.Should().Be(TestUserFlow.AddOld);
        stateMachine.Fire(Trigger.UserWantToContinue);

        stateMachine.State.Should().Be(TestUserFlow.AddName);
    }

    [Test]
    public void CorrectWork_IF_MoveToSwitch()
    {
        var serviceRegistry = collection.BuildServiceProvider().GetService<IServiceRegistryFlow>();
        collection.AddContext<TestUserFlow>("test",
            x => x.AddHandler<FakeHandler2>().AddSwitch<FakeSwitch>(
                (x => x.AddHandler<FakeHandler>(), "whoap")),
            serviceRegistry);

        var stateMachine = new StateMachine<TestUserFlow, Trigger>(TestUserFlow.Authorization);
        serviceRegistry.Wraps(stateMachine);

        stateMachine.Fire(Trigger.UserWantToContinue);
        stateMachine.State.Should().Be(TestUserFlow.AddSecondName);
    }
}

public class FakeHandler : ContextHandler<BasePayload, TestUserFlow>
{
    protected override async Task Handle(Update update, DetailContext<BasePayload, TestUserFlow> context)
    {
        context.State.Continue();
    }

    protected override Task Enter(DetailContext<BasePayload, TestUserFlow> context)
    {
        throw new NotImplementedException();
    }
}

public class FakeSwitch : ContextHandler<BasePayload, TestUserFlow>
{
    protected override async Task Handle(Update update, DetailContext<BasePayload, TestUserFlow> context)
    {
        context.State.Continue();
    }

    protected override Task Enter(DetailContext<BasePayload, TestUserFlow> context)
    {
        throw new NotImplementedException();
    }
}

public class FakeHandler2 : ContextHandler<BasePayload, TestUserFlow>
{
    protected override async Task Handle(Update update, DetailContext<BasePayload, TestUserFlow> context)
    {
        context.State.Continue();
    }

    protected override Task Enter(DetailContext<BasePayload, TestUserFlow> context)
    {
        throw new NotImplementedException();
    }
}