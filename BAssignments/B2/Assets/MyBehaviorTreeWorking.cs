﻿using UnityEngine;
using System.Collections;
using TreeSharpPlus;
using RootMotion.FinalIK;

public class MyBehaviorTreeWorking : MonoBehaviour
{

    public Transform[] locations;
    public GameObject[] numberOfParticipants;
    public FullBodyBipedEffector[] eff;
    public InteractionObject[] obj;
    public InteractionObject[] objReset;
    public InteractionObject objects;
    public InteractionSystem[] interacts;
    public Transform[] searchPoints;


    private BehaviorAgent behaviorAgent;
	// Use this for initialization
	void Start ()
	{
		behaviorAgent = new BehaviorAgent (this.BuildTreeRoot ());
		BehaviorManager.Instance.Register (behaviorAgent);
		behaviorAgent.StartBehavior ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
    

    protected Node Converse(int player1Index, int player2Index)
    {
        return
            new DecoratorPrintResult(
                new Sequence( 
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("ACKNOWLEDGE", AnimationLayer.Face, 1000),
                numberOfParticipants[player2Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("HEADSHAKE", AnimationLayer.Face, 1000),
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("BEINGCOCKY", AnimationLayer.Hand, 1000),
                numberOfParticipants[player2Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("HEADNOD", AnimationLayer.Face, 1000)));
    }

    public Node EyeContact(Val<Vector3> WanderPos, Val<Vector3> FriendPos)
    {
        Vector3 height = new Vector3(0.0f, 1.85f, 0.0f);
        Val<Vector3> WanderHead = Val.V(() => WanderPos.Value + height);
        Val<Vector3> Friendhead = Val.V(() => FriendPos.Value + height);

        return new SequenceParallel(
            numberOfParticipants[0].GetComponent<BehaviorMecanim>().Node_HeadLook(Friendhead),
            numberOfParticipants[1].GetComponent<BehaviorMecanim>().Node_HeadLook(WanderHead));
    }

    public Node EyeContactAndConverse(Val<Vector3> WanderPos, Val<Vector3> FriendPos)
    {
        return new Race(
            this.EyeContact(WanderPos, FriendPos));
            //this.Converse());
    }

    public Node ST_ApproachAndOrient(Transform target1, Transform player1pos, int player1Index , Transform target2, Transform player2pos, int player2Index)
    {
        Val <Vector3> p1pos = Val.V(() => player1pos.position);
        Val<Vector3> p2pos = Val.V(() => player2pos.position);
        Val<Vector3> position1 = Val.V(() => target1.position);
        Val<Vector3> position2 = Val.V(() => target2.position);
        return new Sequence(
            new SequenceParallel(
            numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(position1),
            numberOfParticipants[player2Index].GetComponent<BehaviorMecanim>().Node_GoTo(position2)),
            new SequenceParallel(
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_OrientTowards(p2pos),
                numberOfParticipants[player2Index].GetComponent<BehaviorMecanim>().Node_OrientTowards(p1pos)));
    }

    protected Node ST_pickUpAndEatFood(Val<FullBodyBipedEffector> effector, Val<InteractionObject> obj, Transform objPos, int player1Index, Val<InteractionObject> objReset)
    {
        Val<Vector3> position = Val.V(() => objPos.position);
        return new Sequence(
        numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(position),
        numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_StartInteraction(effector, obj),
        new LeafWait(2000),
        numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("EAT", AnimationLayer.Face, 1000),
        new LeafWait(2000),
        numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_StartInteraction(effector, objReset));
        

    }
    protected Node ST_DanceBattle(int player1Index, int player2Index)
    {
        return
                new Sequence(
                        numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("CROWDPUMP", AnimationLayer.Hand, 1000), 
                        new LeafWait(2000),
                        numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("POINTING", AnimationLayer.Hand, 1000),
                        new LeafWait(2000),
                        numberOfParticipants[player2Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("YAWN", AnimationLayer.Face, 1000),
                        new LeafWait(2000),
                        numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("CUTTHROAT", AnimationLayer.Hand, 1000),
                        new LeafWait(2000),
                        numberOfParticipants[player2Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("HEADSHAKE", AnimationLayer.Face, 1000),
                        new LeafWait(2000),
                        numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("SATNIGHTFEVER", AnimationLayer.Hand, 3000),
                        new LeafWait(2000),
                        numberOfParticipants[player2Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("BREAKDANCE", AnimationLayer.Body, 1000),
                        new LeafWait(2000), 
                        numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("SURPRISED", AnimationLayer.Hand, 2000),
                        new LeafWait(2000), 
                        numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("CLAP", AnimationLayer.Hand, 1000),
                        new LeafWait(2000));

    }

    protected Node ST_ApproachAndWait(int player1Index, Transform target)
	{
		Val<Vector3> position = Val.V (() => target.position);
		return new Sequence(
             numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(position), 
            new LeafWait(1000));

	}

    protected Node ST_ShakeHands(InteractionObject handShake, Val<FullBodyBipedEffector> effector1, Val<FullBodyBipedEffector> effector2)
    {
       
        return new SequenceParallel(
            numberOfParticipants[0].GetComponent<BehaviorMecanim>().Node_StartInteraction(effector1, handShake),
            numberOfParticipants[1].GetComponent<BehaviorMecanim>().Node_StartInteraction(effector2, handShake),
            new LeafWait(1000));
    }

   

    protected Node ST_Gesture(int player1Index, int loop)
    {
        //Val<Vector3> position = Val.V(() => target.position);
        return new DecoratorLoop(loop,
            new Sequence(
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().ST_PlayGesture("CHEER", AnimationLayer.Hand, 1000),
                new LeafWait(1000)));
    }
    protected Node ST_LightSwitch(Val<FullBodyBipedEffector> effector, Val<InteractionObject> obj, int player1Index, Transform objpos)
    {
        Val<Vector3> objPos = Val.V(() => objpos.position);
        return new Sequence(
            numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(objPos),
            numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_StartInteraction(effector, obj),
            this.ST_Gesture(player1Index, 2));

    }
    protected Node ST_Search( int player1Index)
    {
        
        return new Sequence(
            new SequenceShuffle(
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(searchPoints[0].transform.position),
                new LeafWait(1000),
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(searchPoints[1].transform.position),
                new LeafWait(1000),
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(searchPoints[2].transform.position),
                new LeafWait(1000),
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(searchPoints[3].transform.position),
                new LeafWait(1000),
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(searchPoints[4].transform.position),
                new LeafWait(1000),
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(searchPoints[5].transform.position),
                new LeafWait(1000),
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(searchPoints[7].transform.position),
                new LeafWait(1000),
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(searchPoints[8].transform.position),
                new LeafWait(1000),
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(searchPoints[9].transform.position),
                new LeafWait(1000),
                numberOfParticipants[player1Index].GetComponent<BehaviorMecanim>().Node_GoTo(searchPoints[10].transform.position),
                new LeafWait(1000)));
                
    }


    protected Node BuildTreeRoot()
    {
        int[] playerIndex = new int[numberOfParticipants.Length];
        //Val<Vector3>[] playerPos = new Val<Vector3>[numberOfParticipants.Length];
        //reshuffle(numberOfParticipants);
        for (int i = 0; i < playerIndex.Length; i++)
        {
            playerIndex[i] = i;
           
        }
        //reshuffle(searchPoints);

        return
            new Sequence(
               
                new SequenceParallel(
                    new Sequence(
                        this.ST_ApproachAndOrient(locations[0], numberOfParticipants[0].gameObject.transform, playerIndex[0], locations[1], numberOfParticipants[1].gameObject.transform, playerIndex[1]),
                        this.Converse(playerIndex[0], playerIndex[1])),
                    new Sequence(
                        this.ST_ApproachAndOrient(locations[2], numberOfParticipants[2].gameObject.transform, playerIndex[2], locations[3], numberOfParticipants[3].gameObject.transform, playerIndex[3]),
                        this.Converse(playerIndex[2], playerIndex[3]),
                            new SequenceParallel(
                            this.ST_pickUpAndEatFood(eff[0], obj[0], locations[8], playerIndex[2], objReset[0]),
                            this.ST_pickUpAndEatFood(eff[1], obj[1], locations[9], playerIndex[3], objReset[1]),
                            this.ST_ApproachAndOrient(locations[4], numberOfParticipants[0].gameObject.transform, playerIndex[0], locations[5], numberOfParticipants[1].gameObject.transform, playerIndex[1])),
                            new SequenceParallel(
                                this.ST_ApproachAndOrient(locations[6], numberOfParticipants[0].gameObject.transform, playerIndex[2], locations[7], numberOfParticipants[1].gameObject.transform, playerIndex[3]),
                                this.ST_DanceBattle(playerIndex[0], playerIndex[1]),

                                new SequenceParallel(
                                    this.ST_Gesture(playerIndex[2],  10),
                                    this.ST_Gesture(playerIndex[3], 10))))),
                    new Sequence(
                        this.ST_ApproachAndOrient(locations[10], numberOfParticipants[0].gameObject.transform, playerIndex[0], locations[11], numberOfParticipants[1].gameObject.transform, playerIndex[1]),
                        this.ST_ShakeHands(obj[6], eff[1], eff[1])),
                new SequenceParallel(
                    new SequenceShuffle(
                        this.ST_ApproachAndWait(playerIndex[0], searchPoints[0]),
                        this.ST_ApproachAndWait(playerIndex[0], searchPoints[3]),
                        this.ST_ApproachAndWait(playerIndex[0], searchPoints[5])),
                    new SequenceShuffle(
                        this.ST_ApproachAndWait(playerIndex[1], searchPoints[1]),
                        this.ST_ApproachAndWait(playerIndex[1], searchPoints[3]),
                        this.ST_ApproachAndWait(playerIndex[1], searchPoints[2])),
                    new SequenceShuffle(
                        this.ST_ApproachAndWait(playerIndex[2], searchPoints[5]),
                        this.ST_ApproachAndWait(playerIndex[2], searchPoints[7]),
                        this.ST_ApproachAndWait(playerIndex[2], searchPoints[8])),
                    new SequenceShuffle(
                        this.ST_ApproachAndWait(playerIndex[3], searchPoints[9]),
                        this.ST_ApproachAndWait(playerIndex[3], searchPoints[3]),
                        this.ST_ApproachAndWait(playerIndex[3], searchPoints[5]))),
                new SelectorParallel(
                    this.ST_LightSwitch(eff[0], obj[7], playerIndex[0], searchPoints[6]),
                    this.ST_LightSwitch(eff[0], obj[7], playerIndex[1], searchPoints[6]),
                    this.ST_LightSwitch(eff[0], obj[7], playerIndex[2], searchPoints[6]),
                    this.ST_LightSwitch(eff[0], obj[7], playerIndex[3], searchPoints[6])));
                       


    }
    void reshuffle(Transform[] texts)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < texts.Length; t++)
        {
            Transform tmp = texts[t];
            int r = Random.Range(t, texts.Length);
            texts[t] = texts[r];
            texts[r] = tmp;
        }
    }

  

}
