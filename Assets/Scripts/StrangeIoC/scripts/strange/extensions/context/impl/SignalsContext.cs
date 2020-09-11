using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using UnityEngine;

namespace strange.extensions.context.impl
{
    public class SignalsContext : MVCSContext
    {
        public SignalsContext (MonoBehaviour view) : base(view)
        {
        }

        public SignalsContext (MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
        {
        }
		
        // Unbind the default EventCommandBinder and rebind the SignalCommandBinder
        protected override void addCoreComponents()
        {
            base.addCoreComponents();
            injectionBinder.Unbind<ICommandBinder>();
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
        }
        /*
        protected override void mapBindings()
        {
            injectionBinder.Bind<IExampleModel>().To<ExampleModel>().ToSingleton();
            injectionBinder.Bind<IExampleService>().To<ExampleService>().ToSingleton();
			

            mediationBinder.Bind<ExampleView>().To<ExampleMediator>();
			

            commandBinder.Bind<CallWebServiceSignal>().To<CallWebServiceCommand>();
			
            //StartSignal is now fired instead of the START event.
            //Note how we've bound it "Once". This means that the mapping goes away as soon as the command fires.
            commandBinder.Bind<StartSignal>().To<StartCommand>().Once ();
			
            //In MyFirstProject, there's are SCORE_CHANGE and FULFILL_SERVICE_REQUEST Events.
            //Here we change that to a Signal. The Signal isn't bound to any Command,
            //so we map it as an injection so a Command can fire it, and a Mediator can receive it
            injectionBinder.Bind<ScoreChangedSignal>().ToSingleton();
            injectionBinder.Bind<FulfillWebServiceRequestSignal>().ToSingleton();
        }*/
    }
}