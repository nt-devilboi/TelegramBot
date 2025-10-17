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
            x => x.AddHandler<FakeHandler>(x => x.AddSubHandler<FakeHandler>()
                    .AddSubHandler<FakeHandler2>())
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


        states[0].Substates.ToArray()[0].UnderlyingState.Should().Be(TestUserFlow.AddSecondName);
        var userGoToSubTask =
            new StateMachine<TestUserFlow, Trigger>.TriggerWithParameters<string>(Trigger.UserGoToSubTask);

        stateMachine.State.Should().Be(TestUserFlow.Authorization);
        stateMachine.Fire(Trigger.UserCompletedSubTask);

        stateMachine.State.Should().Be(TestUserFlow.AddSecondName);
        stateMachine.Fire(Trigger.UserCompletedSubTask);

        stateMachine.State.Should().Be(TestUserFlow.AddOld);
        stateMachine.Fire(Trigger.UserCompletedAllSubTask);

        stateMachine.State.Should().Be(TestUserFlow.Authorization);
        stateMachine.Fire(userGoToSubTask, TestUserFlow.AddOld.ToString());

        stateMachine.State.Should().Be(TestUserFlow.AddOld);
        stateMachine.Fire(Trigger.UserCompletedAllSubTask);

        stateMachine.Fire(Trigger.UserWantToContinue);
        stateMachine.State.Should().Be(TestUserFlow.AddName);
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