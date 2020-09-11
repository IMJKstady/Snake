using strange.extensions.context.api;
using strange.extensions.mediation.api;
using UnityEngine;
using Photon;

namespace Snake.Online
{
    public class NetworkMediator : PunBehaviour, IMediator
    {

        [Inject(ContextKeys.CONTEXT_VIEW)] 
        public GameObject contextView { get; set; }

        public NetworkMediator()
        {
        }

        /**
         * Fires directly after creation and before injection
         */
        virtual public void PreRegister()
        {
        }

        /**
         * Fires after all injections satisifed.
         *
         * Override and place your initialization code here.
         */
        virtual public void OnRegister()
        {
        }

        /**
         * Fires on removal of view.
         *
         * Override and place your cleanup code here
         */
        virtual public void OnRemove()
        {
        }
    }
}