using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ForestGame : MonoBehaviour {
    public const float Size = 1609.344f;
    public static ForestGame Instance;
    public Transform player; public CharacterController controller; public Camera view;
    public int hp=100, kills, weapon, ammo; public bool playing;
    readonly string[] names={"Pistol","M16","Sniper"};
    readonly int[] capacity={15,30,5}, damage={25,34,90};
    readonly float[] interval={.35f,.12f,1.2f};
    float nextShot, reloadUntil, pitch, vertical; Material wood, leaves, stone, ground, metal;
    GameObject gun; readonly List<ForestBot> bots=new List<ForestBot>();
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] static extern void ForestReady();
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot(){ if(!FindObjectOfType<ForestGame>()) new GameObject("Forest Siege").AddComponent<ForestGame>(); }
    Material Mat(Color c){var m=new Material(Shader.Find("Standard"));m.color=c;m.enableInstancing=true;return m;}
    GameObject Shape(PrimitiveType t,Vector3 p,Vector3 s,Material m,Transform parent=null,bool collider=true){var o=GameObject.CreatePrimitive(t);o.transform.SetParent(parent,false);o.transform.localPosition=p;o.transform.localScale=s;o.GetComponent<Renderer>().sharedMaterial=m;if(!collider)Destroy(o.GetComponent<Collider>());return o;}
    void Start(){Instance=this; Random.InitState(4381);wood=Mat(new Color(.26f,.17f,.09f));leaves=Mat(new Color(.13f,.30f,.12f));stone=Mat(new Color(.37f,.39f,.36f));ground=Mat(new Color(.29f,.37f,.19f));metal=Mat(new Color(.13f,.15f,.16f));
        RenderSettings.ambientLight=new Color(.62f,.65f,.62f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.65f,.73f,.71f);RenderSettings.fogMode=FogMode.Linear;RenderSettings.fogStartDistance=90;RenderSettings.fogEndDistance=260;
        var light=new GameObject("Sun").AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.15f;light.transform.rotation=Quaternion.Euler(48,-35,0);
        Shape(PrimitiveType.Cube,new Vector3(0,-.5f,0),new Vector3(Size,1,Size),ground);
        float h=Size/2;for(int i=-1;i<=1;i+=2){Shape(PrimitiveType.Cube,new Vector3(i*(h+.5f),4,0),new Vector3(1,8,Size+2),stone);Shape(PrimitiveType.Cube,new Vector3(0,4,i*(h+.5f)),new Vector3(Size+2,8,1),stone);}
        Fort(-690);Fort(690);
        for(int i=0;i<2400;i++){float x=Random.Range(-h+8,h-8),z=Random.Range(-h+8,h-8);if(Mathf.Abs(x)<45&&Mathf.Abs(z)>630)continue;if(Mathf.Abs(x)<12)continue;Tree(x,z);}
        for(int i=0;i<1000;i++){float x=Random.Range(140,410),z=Random.Range(-360,-100);Tree(x,z);}
        for(int i=0;i<700;i++)Tree(Random.Range(-490,-240),Random.Range(170,430));
        foreach(var c in FindObjectsOfType<Camera>())c.gameObject.SetActive(false);
        player=new GameObject("Player").transform;player.position=new Vector3(0,1,-650);controller=player.gameObject.AddComponent<CharacterController>();controller.height=1.8f;controller.radius=.35f;controller.center=new Vector3(0,.9f,0);
        view=new GameObject("FPS Camera").AddComponent<Camera>();view.transform.SetParent(player,false);view.transform.localPosition=new Vector3(0,1.65f,0);view.farClipPlane=300;view.fieldOfView=75;view.gameObject.AddComponent<AudioListener>();
        gun=Shape(PrimitiveType.Cube,new Vector3(.27f,-.25f,.62f),new Vector3(.15f,.16f,.55f),metal,view.transform,false);Select(0);
        for(int i=0;i<14;i++)SpawnBot(i);Cursor.lockState=CursorLockMode.None;
#if UNITY_WEBGL && !UNITY_EDITOR
        ForestReady();
#endif
    }
    void Tree(float x,float z){float h=Random.Range(5f,11f);Shape(PrimitiveType.Cylinder,new Vector3(x,h*.28f,z),new Vector3(.55f,h*.28f,.55f),wood);Shape(PrimitiveType.Capsule,new Vector3(x,h*.72f,z),new Vector3(h*.48f,h*.42f,h*.48f),leaves,null,false);}
    void Fort(float z){for(int i=-4;i<=4;i++){if(i!=0)Shape(PrimitiveType.Cube,new Vector3(i*8,2,z-27),new Vector3(7.5f,4,2),stone);if(i%3!=0)Shape(PrimitiveType.Cube,new Vector3(i*8,2,z+27),new Vector3(7.5f,4,2),stone);}for(int i=-3;i<=3;i++){if(i!=1)Shape(PrimitiveType.Cube,new Vector3(-36,2,z+i*8),new Vector3(2,4,7.5f),stone);if(i!=-1)Shape(PrimitiveType.Cube,new Vector3(36,2,z+i*8),new Vector3(2,4,7.5f),stone);}for(int i=0;i<12;i++)Shape(PrimitiveType.Cube,new Vector3(Random.Range(-28,28),.7f,z+Random.Range(-19,19)),new Vector3(3,1.4f,3),stone);}
    void SpawnBot(int i){var b=Shape(PrimitiveType.Capsule,new Vector3(Random.Range(-17,17),1,-600+i*15),new Vector3(.8f,1,.8f),metal,null,false);var cc=b.AddComponent<CharacterController>();cc.height=2;cc.radius=.4f;var bot=b.AddComponent<ForestBot>();bots.Add(bot);}
    void Select(int i){weapon=i;ammo=capacity[i];reloadUntil=0;if(gun)gun.transform.localScale=new Vector3(.14f,.15f,i==0?.28f:i==1?.65f:.9f);}
    void Update(){if(!player)return;if(Input.GetKeyDown(KeyCode.Escape)){playing=false;Cursor.lockState=CursorLockMode.None;}if(!playing)return;
        player.Rotate(0,Input.GetAxis("Mouse X")*2.2f,0);pitch=Mathf.Clamp(pitch-Input.GetAxis("Mouse Y")*2.2f,-85,85);view.transform.localRotation=Quaternion.Euler(pitch,0,0);
        Vector3 move=Vector3.ClampMagnitude(player.right*Input.GetAxisRaw("Horizontal")+player.forward*Input.GetAxisRaw("Vertical"),1);if(controller.isGrounded){vertical=-2;if(Input.GetKeyDown(KeyCode.Space))vertical=6;}vertical-=18*Time.deltaTime;controller.Move((move*(Input.GetKey(KeyCode.LeftShift)?9:5)+Vector3.up*vertical)*Time.deltaTime);
        for(int i=0;i<3;i++)if(Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1+i)))Select(i);
        if(reloadUntil>0&&Time.time>=reloadUntil){ammo=capacity[weapon];reloadUntil=0;}if(Input.GetKeyDown(KeyCode.R)&&reloadUntil==0)reloadUntil=Time.time+1.4f;
        view.fieldOfView=Input.GetMouseButton(1)?(weapon==2?25:52):75;
        if(Input.GetMouseButton(0)&&Time.time>=nextShot&&reloadUntil==0){if(ammo==0){reloadUntil=Time.time+1.4f;return;}ammo--;nextShot=Time.time+interval[weapon];if(Physics.Raycast(view.transform.position,view.transform.forward,out RaycastHit hit,weapon==0?90:280)){var b=hit.collider.GetComponent<ForestBot>();if(b)b.Hurt(damage[weapon]);}}
    }
    public void Hurt(int amount){hp-=amount;if(hp<=0){hp=0;playing=false;Cursor.lockState=CursorLockMode.None;}}
    public void BotKilled(ForestBot b){kills++;bots.Remove(b);Destroy(b.gameObject);if(bots.Count==0){playing=false;Cursor.lockState=CursorLockMode.None;}}
    void OnApplicationFocus(bool focus){AudioListener.pause=!focus;if(!focus){playing=false;Cursor.lockState=CursorLockMode.None;}}
    void OnGUI(){if(!player)return;GUI.skin.label.fontSize=19;GUI.Label(new Rect(15,12,450,35),$"HP {hp}   |   {names[weapon]}  {ammo}/{capacity[weapon]}   |   Kills {kills}/14");if(playing){GUI.Label(new Rect(Screen.width/2-6,Screen.height/2-14,24,28),"+");return;}float x=Screen.width/2-180,y=Screen.height/2-110;GUI.Box(new Rect(x,y,360,220),"FOREST SIEGE");GUI.Label(new Rect(x+20,y+38,320,60),hp==0?"Defeat":bots.Count==0?"Victory":"WASD / Mouse / Shift / Space\n1-3 Weapons / R Reload / RMB Aim");if(GUI.Button(new Rect(x+40,y+115,280,40),hp==0||bots.Count==0?"Restart":"Solo / Resume")){if(hp==0||bots.Count==0){foreach(var b in bots)if(b)Destroy(b.gameObject);bots.Clear();hp=100;kills=0;controller.enabled=false;player.position=new Vector3(0,1,-650);controller.enabled=true;for(int i=0;i<14;i++)SpawnBot(i);Select(0);}playing=true;Cursor.lockState=CursorLockMode.Locked;}}
}
