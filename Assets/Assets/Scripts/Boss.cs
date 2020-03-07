using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    // ヒットポイント
    public int hp = 100;
    public int Maxhp;
    // スコアのポイント
    public int point = 100;

    // コンポーネント
    Spaceship spaceship;
    LineRenderer hpBar;
    //GameObject player;

    void Start() 
    {
        Maxhp = hp;
        // コンポーネントを取得
        spaceship = GetComponent<Spaceship>();
        hpBar = GetComponent<LineRenderer>();
    }

    void Update() 
    {
        transform.Translate(Mathf.Sin(Time.time) * 0.02f, 0, 0);
        if (spaceship.canShot == true)
        {
            StartCoroutine("Shot_Enemy");
            spaceship.canShot = false;
        }
    }

    // 機体の移動
    public void Move(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().velocity = direction * spaceship.speed;
    }

    IEnumerator Shot_Enemy()
    {

        // ローカル座標のY軸のマイナス方向に移動する
        //Move(transform.up * -1);
        // canShotがfalseの場合、ここでコルーチンを終了させる
        while (true)
        {
            // 子要素を全て取得する
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform shotPosition = transform.GetChild(i);
                // ShotPositionの位置/角度で弾を撃つ
                spaceship.Shot1(shotPosition);
                shotPosition.transform.Rotate(0, 0, Mathf.Sin(Time.time) * 3);
            }
            // shotDelay秒待つ
            yield return new WaitForSeconds(spaceship.shotDelay);
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        // レイヤー名を取得
        string layerName = LayerMask.LayerToName(c.gameObject.layer);

        // レイヤー名がBullet (Player)以外の時は何も行わない
        if (layerName != "Bullet (Player)") return;

        // PlayerBulletのTransformを取得
        Transform playerBulletTransform = c.transform.parent;

        // Bulletコンポーネントを取得
        Bullet bullet = playerBulletTransform.GetComponent<Bullet>();

        // ヒットポイントを減らす
        hp -= bullet.power;

        // 弾の削除
        Destroy(c.gameObject);

        if (hp >= 0)
        {
            hpBar.SetPosition(1, new Vector3(3 * (hp / Maxhp), 2.5f, -55f));
        }
        // ヒットポイントが0以下であれば
        if (hp <= 0)
        {
            // スコアコンポーネントを取得してポイントを追加
            FindObjectOfType<Score>().AddPoint(point);

            // 爆発
            spaceship.Explosion();

            // エネミーの削除
            Destroy(gameObject);

        }
        else
        {

            spaceship.GetAnimator().SetTrigger("Damage");

        }
    }

    void OnAnimationFinish()
    {
        spaceship.canShot = true;
    }
}