#include <stdio.h>
#include <math.h>
double integrate(int n)
{
    int i;
    const double h = 0.5/n;
    const double h2 = 0.5*h;
    double k = 0.0;
    for (i = 0; i < n; i++)
        {
            double xi = i*h + h2;
            if (i <= (n / 2))
                k += exp(sin(xi));
            else
                k += exp(xi) - 1/(sqrt(xi)); 
        }
    return k*h;
}
int main()
{
    unsigned int n = 1000;
    double e;
    printf ("Введите точность -> ");
    scanf("%lf", &e);
    double integrate1 = integrate(n);
    double integrate2 = integrate(n*2);
    while (fabs((integrate2 - integrate1)/3) >= e)  
    {
        n *= 2;
        integrate1 = integrate2;
        integrate2 = integrate(n*2);
    }              
    printf("Контрольное значение интеграла: 0,23431.\nИтоговое значение интеграла: %.5lf\n", integrate2);
}