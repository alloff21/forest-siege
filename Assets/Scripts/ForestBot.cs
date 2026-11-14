using UnityEngine;
public class ForestBot:MonoBehaviour {
 int hp=100;float nextAttack;CharacterController cc;
 void Start(){cc=GetComponent<CharacterController>();}
 public void Hurt(int damage){hp-=damage;if(hp<=0)ForestGame.Instance.BotKilled(this);}
 void Update(){var g=ForestGame.Instance;if(!g||!g.playing||!g.player)return;Vector3 delta=g.player.position-transform.position;delta.y=0;float d=delta.magnitude;if(d>180)return;Vector3 dir=delta.normalized;if(d>12)cc.Move((dir*3.6f+Vector3.down*2)*Time.deltaTime);if(d<65&&Time.time>nextAttack){nextAttack=Time.time+Random.Range(.8f,1.5f);Vector3 start=transform.position+Vector3.up*.7f;Vector3 target=g.view.transform.position;if(!Physics.Linecast(start+dir*.6f,target,out RaycastHit h)||h.collider.transform==g.player)g.Hurt(5);}}
}
