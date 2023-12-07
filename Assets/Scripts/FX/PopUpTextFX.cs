using TMPro;
using UnityEngine;

public class PopUpTextFX : MonoBehaviour
{
    private TextMeshPro myText;

    [SerializeField] private float speed;
    [SerializeField] private float dissapearSpeed;
    [SerializeField] private float colorDissapearSpeed;
    [SerializeField] private float lifeTime;

    private float textTimer;

    private void Start()
    {
        myText = GetComponent<TextMeshPro>();
        textTimer = lifeTime;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y + 0.8f), speed * Time.deltaTime);
        textTimer -= Time.deltaTime;

        if (textTimer <= 0)
        {
            float alpha = myText.color.a - colorDissapearSpeed * Time.deltaTime;
            myText.color = new Color(myText.color.r, myText.color.g, myText.color.b, alpha);

            if (myText.color.a < 50)
                speed = dissapearSpeed;

            if (myText.color.a <= 0)
                Destroy(gameObject);
        }
    }
}
