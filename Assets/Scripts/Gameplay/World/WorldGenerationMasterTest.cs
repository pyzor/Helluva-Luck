using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerationMasterTest : MonoBehaviour {

    [SerializeField] private int Seed;





    void Start() {
        NormalTileBehavior.Probability = 3;
        CornerTileBehavior.Probability = 3;
        GreedBankTileBehavior.Probability = 1;
        PrideFightTileBehavior.Probability = 1;
        SlothAutoMoveTileBehavior.Probability = 1;
        ChangeDirectionTileBehavior.Probability = 1;
        GluttonyFreeRewardTileBehavior.Probability = 1;
        WrathFightTileBehavior.Probability = 1;
        EnvyShopTileBehavior.Probability = 1;
        LustTileBehavior.Probability = 1;

        WorldGenerationMaster.Instance.AddNewBehavior(new NormalTileBehavior());
        WorldGenerationMaster.Instance.AddNewBehavior(new GreedBankTileBehavior());
        WorldGenerationMaster.Instance.AddNewBehavior(new PrideFightTileBehavior());
        WorldGenerationMaster.Instance.AddNewBehavior(new SlothAutoMoveTileBehavior());
        WorldGenerationMaster.Instance.AddNewBehavior(new ChangeDirectionTileBehavior());
        WorldGenerationMaster.Instance.AddNewBehavior(new CornerTileBehavior());
        WorldGenerationMaster.Instance.AddNewBehavior(new GluttonyFreeRewardTileBehavior());
        WorldGenerationMaster.Instance.AddNewBehavior(new WrathFightTileBehavior());
        WorldGenerationMaster.Instance.AddNewBehavior(new EnvyShopTileBehavior());
        WorldGenerationMaster.Instance.AddNewBehavior(new LustTileBehavior());

        WorldGenerationMaster.Instance.Init(Seed);
    }

}
