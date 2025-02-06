using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//Fazer o player tomar hit e implementar os inimigos
public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] float move_speed;
    [SerializeField] float Dash_force;
    [SerializeField] GameObject GunPivot;
    [SerializeField] GameObject Crosshair;
    [SerializeField] GameObject Bullet;
    [SerializeField] float shotGunrecoil;
    [SerializeField] TMP_Text Ammo;
    [SerializeField] Image reloadImage;
    Vector2 moveInput;
    [SerializeField]float duration = 0.25f;
    [SerializeField]float pistolCoolDown = 0.5f;
    [SerializeField]int PistolBullets = 1;
    [SerializeField]int PistolMags = 60;
    [SerializeField]int PistolMaxAmmo = 20;
    [SerializeField]int ShotgunAmmo = 12;
    [SerializeField]int ShotgunMags = 36;
    [SerializeField]int ShotgunMaxAmmo = 12;
    [SerializeField]float PistolReloadCD = 1f;
    int currentGunAmmo;
    int currentGunMags;
    int currentGunMaxAmmo;
    float shootTime;
    bool can_shoot = true;
    bool is_reloading = false;
    int weapon_type = 0;
    Vector2 mouseWPosition;
    Vector2 dirMouse;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mouseWPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dirMouse = new Vector2(mouseWPosition.x - GunPivot.transform.position.x, mouseWPosition.y - GunPivot.transform.position.y);
        //print(PistolBullets);
        Ammo.text = currentGunAmmo + "/" + currentGunMags;
        if(Input.GetMouseButtonDown(1)){
            StartCoroutine(recoil(rb.velocity, Dash_force));
        }
        _movement();
        _gun_control();
    }
    void _movement(){
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
        rb.velocity = moveInput * move_speed;  
    }
 
    void _gun_control(){

        float angle = Mathf.Atan2(dirMouse.y, dirMouse.x) * Mathf.Rad2Deg;
        GunPivot.transform.rotation = Quaternion.Euler(0,0,angle);
        if(Input.GetMouseButtonDown(0) && can_shoot && weapon_type == 0 && currentGunAmmo>0 && !is_reloading){
            Instantiate(Bullet, Crosshair.transform.position, GunPivot.transform.rotation);
            shootTime = Time.time;
            can_shoot = false;
            PistolBullets--;
        }
        else if(Input.GetMouseButtonDown(0) && can_shoot && weapon_type == 1 && currentGunAmmo>0 && !is_reloading){
            Vector3 gunAngle = GunPivot.transform.rotation.eulerAngles;
            Vector3 rotation1 = new Vector3(gunAngle.x,gunAngle.y,gunAngle.z +30);
            Vector3 rotation2 = new Vector3(gunAngle.x,gunAngle.y,gunAngle.z-30);
            Instantiate(Bullet, Crosshair.transform.position, Quaternion.Euler(rotation1));
            Instantiate(Bullet, Crosshair.transform.position, GunPivot.transform.rotation);
            Instantiate(Bullet, Crosshair.transform.position, Quaternion.Euler(rotation2));
            shootTime = Time.time;
            can_shoot = false;
            StartCoroutine(recoil(-dirMouse, shotGunrecoil));
            ShotgunAmmo -= 3;
            
        }
            
        if(Time.time - shootTime >= pistolCoolDown){
            can_shoot = true;
        }
        //ChangeGun
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            weapon_type = 0;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            weapon_type = 1;
        }
        if(weapon_type == 0){
            currentGunAmmo = PistolBullets;
            currentGunMags = PistolMags;
            currentGunMaxAmmo = PistolMaxAmmo;
        }else if(weapon_type == 1){
            currentGunAmmo = ShotgunAmmo;
            currentGunMags = ShotgunMags;
            currentGunMaxAmmo = ShotgunMaxAmmo;
        }
        //reloading
        if(currentGunAmmo == 0 && currentGunMags >0 || currentGunAmmo < currentGunMaxAmmo && Input.GetKeyDown(KeyCode.R)){
            StartCoroutine(Reload());
        }
        IEnumerator Reload(){
            //print("Recarregando");
            is_reloading = true;
            float timer = 0f;
            while(timer < PistolReloadCD){
                reloadImage.fillAmount = timer/PistolReloadCD;
                timer+= Time.deltaTime;
                yield return null;
            }
            while(currentGunAmmo<currentGunMaxAmmo && currentGunMags > 0){
                currentGunMags--;
                currentGunAmmo++;
            }
            if(weapon_type == 0){
                PistolBullets = currentGunAmmo;
                PistolMags = currentGunMags;
            }else if(weapon_type == 1){
                ShotgunAmmo = currentGunAmmo;
                ShotgunMags = currentGunMags;
            }
            //print("Recarregado!");
            is_reloading = false;
        }
        
    }//Criar uma função para produzir empurrões (Dashs, coices etc)
   IEnumerator recoil(Vector2 offset, float intensity){
        float timer =0f;
        while(timer < duration){
            rb.velocity = offset.normalized * intensity;
            timer+= Time.deltaTime;
            yield return null;
        }
        rb.velocity = Vector2.zero;
   }
}
