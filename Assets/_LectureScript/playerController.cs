using Unity.Jobs;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class playerController : MonoBehaviour, IDamage, IPickup
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask shootLayer;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int springMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;

    int jumpCount;
    int HPOrig;
    int gunListPos;


    float shootTimer;

    Vector3 moveDir;
    Vector3 playerVel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {

        if (!gamemanager.instance.isPaused)
        {
            movement();
        }

        sprint();
    }

    void movement()
    {
        if (gunList.Count > 0)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * gunList[gunListPos].shootDist, Color.red);
        }

        shootTimer += Time.deltaTime;

        if(Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer > gunList[gunListPos].shootRate)
        shoot();

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel.y = 0;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir.normalized * speed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime);
        
        playerVel.y -= gravity * Time.deltaTime;

        selectgun();
        reload();
    }

    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            speed *= springMod;
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            speed /= springMod;
        }
    }
    void jump()
    {
        if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        gunList[gunListPos].ammoCur--;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, gunList[gunListPos].shootDist, shootLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponentInParent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(gunList[gunListPos].shootDamage);
            }
        }
    }

    void reload()
    {
        if(Input.GetButtonDown("Reload") && gunList.Count > 0)
        {
            gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        }
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();

        StartCoroutine(flashDamageScreen());

        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageScreen.SetActive(false);
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;
        changeGun();
        
    }

    void changeGun()
    {
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectgun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }
}
