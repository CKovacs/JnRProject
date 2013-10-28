using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    public GameObject target;
    public float attackRange = 3;
    public float attackRangeRange = 10;
    public float attackTimer;
    public float selectTimer;
    public float coolDown;
    public GameObject TargetsGo;
    public GameObject selectedTarget;

    public GameObject _instancesTargetLock;
    int max = -1;
    int currentIndex = -1;

    public GameObject _projectilePrefab;
    // Use this for initialization
    void Start()
    {
        attackTimer = 0;
        selectTimer = 0;
        coolDown = 0.10f; //in seconds    
    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime; //in sec	
        }

        if (attackTimer < 0)
        {
            attackTimer = 0;
        }

        if (selectTimer > 0)
        {
            selectTimer -= Time.deltaTime; //in sec	
        }

        if (selectTimer < 0)
        {
            selectTimer = 0;
        }

        if (Input.GetButton("AttackMelee") && attackTimer == 0)
        {
            AttackMeele();
            attackTimer = coolDown;
        }

        if ((Input.GetButton("AttackRange") || Input.GetKeyUp(KeyCode.E)) && attackTimer == 0)
        {
            StartCoroutine(AttackRange());
            attackTimer = coolDown * 4;
        }

        if (Input.GetKeyUp(KeyCode.G) && attackTimer == 0)
        {
            StartCoroutine(AttackRange());
            attackTimer = coolDown;
        }

        if ((Input.GetButton("SelectTargetRight") || Input.GetKeyUp(KeyCode.Tab)) && selectTimer == 0)
        {
            GetTarget();
            selectTimer = coolDown * 2;
        }
    }

    private void AttackMeele()
    {
        TargetsGo = GameObject.Find("Targets");
        var targetScript = TargetsGo.GetComponent("SelectTarget") as SelectTarget;
        for (int i = 0; i < targetScript._visibleTargets.Count; i++)
        {
            float distance = Vector3.Distance(targetScript._visibleTargets[i].position, transform.position);
            if (distance <= attackRange)
            {
                Vector3 dir = (targetScript._visibleTargets[i].position - transform.position).normalized;
                float direction = Vector3.Dot(dir, transform.forward);
                if (direction > 0.6)
                {
                    //blabla
                    //for each taget adjust health
                    Debug.Log("Attack" + targetScript._visibleTargets[i].gameObject);

                    animation.Play("attack");//Play("attack");
                }
            }
        }
    }

    private IEnumerator AttackRange()
    {
        



        if (selectedTarget != null)
        {
            float distance = Vector3.Distance(selectedTarget.transform.position, transform.position);
            if (distance <= attackRangeRange)
            {
                Vector3 dir = (selectedTarget.transform.position - transform.position).normalized;
                float direction = Vector3.Dot(dir, transform.forward);
                if (direction > 0)
                {


                    GameObject projectile = GameObject.Instantiate(_projectilePrefab) as GameObject;

                    Vector3 newPos = new Vector3(this.transform.position.x, this.transform.position.y + 1.0f, this.transform.position.z);


                    Vector3 newSource = this.transform.position;
                    newSource += new Vector3(0, 2, 0);

                    projectile.transform.position = newSource;

                    Vector3 newTarget = selectedTarget.transform.position;

                    newTarget += new Vector3(0, 0.5f, 0);

                    Vector3 position = Vector3.Lerp(projectile.transform.position, newTarget, Time.deltaTime * 0.1f);
                    projectile.transform.position = position;
                    Destroy(projectile, 5.5f);
                    projectile.rigidbody.AddRelativeForce((newTarget - this.transform.position) * 200);

                    foreach (Transform t in selectedTarget.transform)
                    {
                        Animation a = t.GetComponent<Animation>();

                        if (a != null)
                        {
                            a.Play("gethit");
                        }
                    }

                    yield return null;
                }
            }

        }

        yield return null;
    }

    private void GetTarget()
    {
        TargetsGo = GameObject.Find("Targets");
        var targetScript = TargetsGo.GetComponent("SelectTarget") as SelectTarget;

        if (targetScript.changed)
        {
            max = targetScript._visibleTargets.Count - 1;
            if (selectedTarget != null)
            {
                currentIndex = -1;
                for (int i = 0; i <= max; i++)
                {
                    if (selectedTarget == targetScript._visibleTargets[i].gameObject)
                    {
                        currentIndex = i;
                    }
                }
                if (currentIndex == -1)
                {
                    selectedTarget = null;

                    _instancesTargetLock.transform.position = new Vector3(100.0f, 100.0f, 100.0f);
                }
            }
            targetScript.changed = false;
        }

        if (selectedTarget == null)
        {
            if (targetScript._visibleTargets.Count != 0)
            {
                selectedTarget = targetScript._visibleTargets[0].gameObject;

                _instancesTargetLock.transform.position = (selectedTarget.transform.position + new Vector3(0.0f, 0.1f, 0.0f));

                currentIndex = 0;
            }
        }
        else
        {
            if (targetScript._visibleTargets.Count != 0)
            {
                if (currentIndex < max)
                {
                    currentIndex++;
                }
                else
                {
                    currentIndex = 0;
                }
                selectedTarget = targetScript._visibleTargets[currentIndex].gameObject;

                _instancesTargetLock.transform.position = (selectedTarget.transform.position + new Vector3(0.0f, 0.1f, 0.0f));
            }
            else
            {
                selectedTarget = null;
                _instancesTargetLock.transform.position = new Vector3(100.0f, 100.0f, 100.0f);
                currentIndex = -1;
            }
        }
    }
}
