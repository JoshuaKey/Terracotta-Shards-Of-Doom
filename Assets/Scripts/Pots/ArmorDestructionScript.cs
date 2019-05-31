using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorDestructionScript : MonoBehaviour
{
	[SerializeField] float PieceDisappearTime = 0.5f;
	bool bossArmor = false;
	bool normalArmor = false;

    // Start is called before the first frame update
    void Start()
    {
		bossArmor = (this.GetComponentInParent<Boss1Pot>() == null) ? false : true;
		normalArmor = (this.GetComponentInParent<Attack>() == null) ? false : true;
    }

    // Update is called once per frame
    void Update()
    {
        if(bossArmor && this.GetComponentInParent<Boss1Pot>() == null)
		{
			StartCoroutine(Disappear());
		}
		else if(normalArmor && this.GetComponentInParent<Attack>() == null)
		{
			StartCoroutine(Disappear());
		}
    }

	private IEnumerator Disappear()
	{
		WaitForSeconds wait = new WaitForSeconds(PieceDisappearTime);
		yield return wait;
		yield return StartCoroutine(ShrinkShards(this.gameObject));
		Destroy(this.gameObject);
	}

	private IEnumerator ShrinkShards(GameObject shard)
	{
		float currentScale = shard.transform.localScale.x;

		while (currentScale >= 0.1f)
		{
			currentScale -= Time.deltaTime;
			shard.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
			yield return null;
		}
		yield break;
	}
}
