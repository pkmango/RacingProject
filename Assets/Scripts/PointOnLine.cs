using UnityEngine;

public class PointOnLine
{
    // Этот класс используется для расчета координат ближайшей точки на прямой 
    // Метод получает координаты начала отрезка posA, конца отрезка posB, начальной точки posC
    // Прямая на плоскости задана уравнением ax + by + c = 0
    // Уравнение прямой, проходящей через две заданные точки(x1, y1) и(x2, y2), имеет вид: (y1-y2)x+(x2-x1)y+(x1y2-x2y1)=0
    // Соответсвтенно находим a, b, c и подставляем в формулу для расчета координат ближайшей точки:
    // x = (b*(b*x0 - a*yo) - a*c) / (a*a + b*b)
    // y = (a*(-b*x0 + a*yo) - b*c) / (a*a + b*b)

    private float maxHight = 0f; // Задаем предельную начальную высоту для респауна

    public Vector3 GetPointOnLine(Vector3 posA, Vector3 posB, Vector3 posC)
    {
        float a = posA.z - posB.z;
        float b = posB.x - posA.x;
        float c = posA.x * posB.z - posB.x * posA.z;
        float x = (b * (b * posC.x - a * posC.z) - a * c) / (a * a + b * b);
        float y = (a * (-b * posC.x + a * posC.z) - b * c) / (a * a + b * b);
        Vector3 spawnPoint = new Vector3(x, maxHight, y);

        // Проверяем находится ли точка внутри заданного отрезка
        float ABLength = Mathf.Round((posB - posA).magnitude); // Длина отрезка между чекпоинтами
        float totalLength = Mathf.Round((spawnPoint - posA).magnitude + (posB - spawnPoint).magnitude); // Сумма отрезков между чекпоинтами и объектом
        if (ABLength < totalLength)
        {
            spawnPoint = posA;
            //Debug.Log(ABLength + " < " + totalLength + " Точка респауна за пределами трассы!");
        }

        return spawnPoint;
    }
}
