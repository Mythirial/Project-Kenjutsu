using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts
{
    public class ClimbInteractable : XRBaseInteractable
    {
        protected override void OnSelectEnter(XRBaseInteractor interactor)
        {
            base.OnSelectEnter(interactor);
            if(interactor is XRDirectInteractor)
                ClimbingMovement.climbingHand = interactor.GetComponent<XRController>();
        }

        protected override void OnSelectExit(XRBaseInteractor interactor)
        {
            base.OnSelectExit(interactor);
            if(interactor is XRDirectInteractor)
                if (ClimbingMovement.climbingHand && ClimbingMovement.climbingHand.name == interactor.name)
                    ClimbingMovement.climbingHand = null;
        }
    }
}
