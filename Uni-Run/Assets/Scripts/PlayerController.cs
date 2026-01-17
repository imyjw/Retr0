using UnityEngine;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour {
   public AudioClip deathClip; // 사망시 재생할 오디오 클립
   public float jumpForce = 700f; // 점프 힘

   private int jumpCount = 0; // 누적 점프 횟수
   private bool isGrounded = false; // 바닥에 닿았는지 나타냄
   private bool isDead = false; // 사망 상태

   private Rigidbody2D playerRigidbody; // 사용할 리지드바디 컴포넌트
   private Animator animator; // 사용할 애니메이터 컴포넌트
   private AudioSource playerAudio; // 사용할 오디오 소스 컴포넌트

   private void Start() {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
       // 초기화
   }

   private void Update() {
        // 사용자 입력을 감지하고 점프하는 처리
        if (isDead)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0) && jumpCount<2)
        {
            jumpCount++;
            playerRigidbody.linearVelocity = Vector2.zero; //점프 직전에 속도를 순간적으로 제로(0,0)로 변경
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            playerAudio.Play();
        }
        else if(Input.GetMouseButtonUp(0) && playerRigidbody.linearVelocity.y > 0) //마우스 왼쪽 버튼에서 손을 떼는 순간 && 위로 상승 중이라면
        {
            playerRigidbody.linearVelocity = playerRigidbody.linearVelocity * 0.5f; //현재 속도를 절반으로 변경. 오래 누르면 높이 점프하도록
        }

        animator.SetBool("Grounded", isGrounded);
   }

   private void Die() {
        animator.SetTrigger("Die");

        playerAudio.clip = deathClip;

        playerAudio.Play();

        playerRigidbody.linearVelocity = Vector2.zero;
        isDead = true;

        GameManager.instance.OnPlayerDead();
   }

   private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag=="Dead" && !isDead)
        {
            Die();
        }
       // 트리거 콜라이더를 가진 장애물과의 충돌을 감지
   }

   private void OnCollisionEnter2D(Collision2D collision) {//OnCollision계열의 충돌 이벤트 메서드는 여러 충돌 정보를 담는 Collision 타입의 데이터를 입력받는다. Collision 타입은 충돌 지점들의 정보를 담는 ContactPoint 타입의 데이터를 배열로서 contacts라는 변수로 제공한다. 따라서 contacts 배열의 길이는 충돌 지점의 개수와 일치한다. 즉, collision.contacts[0]은 두 물체 사이의 여러 충돌 지점 중에서 첫번째 충돌 지점의 정보를 가져온 것이다. ContactPoint, ContactPoint2D타입은 충돌지점에서 충돌 표면의 방향(노멀벡터)를 알려주는 변수인 normal을 제공한다. 어떤 표면의 노멀벡터의 y값이 1.0인 경우 해당 표면의 방향은 위쪽, 0이면 완전히 오른쪽이나 왼쪽, -1.0이면 아래를 향한다. 0.7이라면 대략 45도의 경사를 가진 채 표면이 위로 향한다. 이 조건을 검사함으로서 절벽이나 천장을 바닥으로 인식하는 문제를 해결.
        if (collision.contacts[0].normal.y > 0.7f) //어떤 콜라이더와 닿았으며, 충돌 표면이 위쪽을 보고 있으면
        {
            isGrounded = true;
            jumpCount = 0;
        }
       // 바닥에 닿았음을 감지하는 처리
   }

   private void OnCollisionExit2D(Collision2D collision) {
        isGrounded = false;
       // 바닥에서 벗어났음을 감지하는 처리
   }
}