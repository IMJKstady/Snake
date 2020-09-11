using Photon;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.mediation.api;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.Networking;
using MonoBehaviour = UnityEngine.MonoBehaviour;

namespace Snake.Online
{
    public class NetworkView : PunBehaviour, IView
    {
        private bool _requiresContext = true;
		public bool requiresContext
		{
			get
			{
				return _requiresContext;
			}
			set
			{
				_requiresContext = value;
			}
		}
		
		protected bool registerWithContext = true;
		virtual public bool autoRegisterWithContext
		{
			get { return registerWithContext;  }
			set { registerWithContext = value; }
		}

		public bool registeredWithContext{get; set;}
		
		protected virtual void Awake ()
		{
			if (autoRegisterWithContext && !registeredWithContext)
				bubbleToContext(this, true, false);
		}

		/// A MonoBehaviour Start handler
		/// If the View is not yet registered with the Context, it will 
		/// attempt to connect again at this moment.
		protected virtual void Start ()
		{
			if (autoRegisterWithContext && !registeredWithContext)
				bubbleToContext(this, true, true);
		}

		/// A MonoBehaviour OnDestroy handler
		/// The View will inform the Context that it is about to be
		/// destroyed.
		protected virtual void OnDestroy ()
		{
			bubbleToContext(this, false, false);
		}

		/// Recurses through Transform.parent to find the GameObject to which ContextView is attached
		/// Has a loop limit of 100 levels.
		/// By default, raises an Exception if no Context is found.
		virtual protected void bubbleToContext(MonoBehaviour view, bool toAdd, bool finalTry)
		{
			const int LOOP_MAX = 100;
			int loopLimiter = 0;
			Transform trans = view.gameObject.transform;
			while(trans.parent != null && loopLimiter < LOOP_MAX)
			{
				loopLimiter ++;
				trans = trans.parent;
				if (trans.gameObject.GetComponent<ContextView>() != null)
				{
					ContextView contextView = trans.gameObject.GetComponent<ContextView>() as ContextView;
					if (contextView.context != null)
					{
						IContext context = contextView.context;
						if (toAdd)
						{
							context.AddView(view);
							registeredWithContext = true;
							return;
						}
						else
						{
							context.RemoveView(view);
							return;
						}
					}
				}
			}
			if (requiresContext && finalTry)
			{
				//last ditch. If there's a Context anywhere, we'll use it!
				if (Context.firstContext != null)
				{
					Context.firstContext.AddView (view);
					registeredWithContext = true;
					return;
				}

				
				string msg = (loopLimiter == LOOP_MAX) ?
					msg = "A view couldn't find a context. Loop limit reached." :
						msg = "A view was added with no context. Views must be added into the hierarchy of their ContextView lest all hell break loose.";
				msg += "\nView: " + view.ToString();
				throw new MediationException(msg,
					MediationExceptionType.NO_CONTEXT);
			}
		}
    }
}