using System.ComponentModel;
Console.OutputEncoding = System.Text.Encoding.UTF8;

bool b = false;
bool FI = false;
bool DI = false;

do
{
    Console.Clear();
    Console.Write("Adja meg, hogy diszkrét vagy folytonos idejű a rendszer! [DI,FI]: ");

    string? line = Console.ReadLine();

    if (string.IsNullOrEmpty(line) == false)
    {
        switch (line)
        {
            case "FI":
            case "fi":
                FI = true;
                b = true;
                break;
            case "DI":
            case "di":
                DI = true;
                b = true;
                break;
            default:
                break;
        }
    }

} while (b != true);

b = false;
Values data = new Values();
string[] variables = { "a00", "a10", "a01", "a11", "b0", "b1", "c0", "c1", "d0", "l0", "l1" };

for(int i = 0; i < variables.Length; ++i)
{
    do
    {
        Console.Clear();
        data.Print();
        data.PrintAlreadyGiven(i, variables);
        Console.Write("Adja meg {0}-t: ", variables[i]);

        string? line = Console.ReadLine();

        if (string.IsNullOrEmpty(line) == false && data.CanCovert(line, typeof(double)) == true)
        {
            data.Initialize(i, Convert.ToDouble(line));
            b = true;
        }

    } while (b != true);

    b = false;
}

Console.Clear();
data.Print();
data.PrintAlreadyGiven(variables.Length, variables);

data.PrintResult(FI, DI);
data.PrintLagrangeMatrices();

class Values
{
    double[] values = new double[11];
    double[,] matrix_a = new double[2, 2];
    double[] cT = new double[2];
    double[] B = new double[2];
    double D;
    double lambda1;
    double lambda2;
    double[,] langrange1 = new double[2, 2];
    double[,] langrange2 = new double[2, 2];

    public void CalculateLagrangeMatrices()
    {
        langrange1[0, 0] = (matrix_a[0, 0] - lambda2) / (lambda1 - lambda2);
        langrange1[0, 1] = matrix_a[0, 1] / (lambda1 - lambda2);
        langrange1[1, 0] = matrix_a[1, 0] / (lambda1 - lambda2);
        langrange1[1, 1] = (matrix_a[1, 1] - lambda2) / (lambda1 - lambda2);

        langrange2[0, 0] = (matrix_a[0, 0] - lambda1) / (lambda2 - lambda1);
        langrange2[0, 1] = matrix_a[0, 1] / (lambda2 - lambda1);
        langrange2[1, 0] = matrix_a[1, 0] / (lambda2 - lambda1);
        langrange2[1, 1] = (matrix_a[1, 1] - lambda1) / (lambda2 - lambda1);
    }

    public double CalculateMConstants(int index)
    {
        if(index == 1)
        {
            double M1 = (cT[0] * langrange1[0, 0] + cT[1] * langrange1[1, 0]) * B[0] + (cT[0] * langrange1[0, 1] + cT[1] * langrange1[1, 1]) * B[1];
            return M1;
        }
        
        double M2 = (cT[0] * langrange2[0, 0] + cT[1] * langrange2[1, 0]) * B[0] + (cT[0] * langrange2[0, 1] + cT[1] * langrange2[1, 1]) * B[1];
        return M2;
    }

    public bool CanCovert(string value, Type type)
    {
        TypeConverter converter = TypeDescriptor.GetConverter(type);
        return converter.IsValid(value);
    }

    public void Initialize(int index, double value)
    {
        switch(index)
        {
            case 0:
                matrix_a[0, 0] = value;
                break;
            case 1:
                matrix_a[1, 0] = value;
                break;
            case 2:
                matrix_a[0, 1] = value;
                break;
            case 3:
                matrix_a[1, 1] = value;
                break;
            case 4:
                B[0] = value;
                break;
            case 5:
                B[1] = value;
                break;
            case 6:
                cT[0] = value;
                break;
            case 7:
                cT[1] = value;
                break;
            case 8:
                D = value;
                break;
            case 9:
                lambda1 = value;
                break;
            case 10:
                lambda2 = value;
                break;
        }

        values[index] = value;
    }

    public void PrintAlreadyGiven(int index, string[] array)
    {
        Console.Write(" {");

        for(int i = 0; i < index; ++i)
        {
            if(i == 0)
            {
                Console.Write(" {0} = {1}", array[i], values[i]);
            }
            else
            {
                Console.Write("; {0} = {1}", array[i], values[i]);
            }          
        }

        Console.WriteLine(" }");
        Console.WriteLine();
    }

    public void Print()
    {
        Console.WriteLine();
        Console.WriteLine(" |‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾|‾‾‾‾‾‾‾‾‾‾‾‾|‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾|‾‾‾‾‾‾‾‾|‾‾‾‾‾‾‾‾‾|");
        Console.WriteLine(@" | A=[ {{{0}}} {{{1}}} ] | B=[ {{{2}}} ] | cT=[ {{{3}}} {{{4}}} ] | D={{{5}}} | λ1={{{6}}} |", "a00", "a01", "b0", "c0", "c1", "d0", "l0");
        Console.WriteLine(@" |   [ {{{0}}} {{{1}}} ] |   [ {{{2}}} ] |                  |        | λ2={{{3}}} |", "a10", "a11", "b1", "l1");
        Console.WriteLine(" |___________________|____________|__________________|________|_________|");
        Console.WriteLine();
    }

    public void PrintResult(bool FI, bool DI)
    {
        CalculateLagrangeMatrices();
        double M1 = CalculateMConstants(1);
        double M2 = CalculateMConstants(2);

        if (FI == true)
        {
            Console.WriteLine("Impulzusválasz FI-ben: h(t)={0}*δ(t)+ε(t)*[{1:0.0000}*e^({2}*t)+{3:0.0000}*e^({4}*t)]", D, M1, lambda1, M2, lambda2);
        }
        else if (DI == true)
        {
            Console.WriteLine("Impulzusválasz DI-ben: h[k]={0}*δ[k]+ε[k−1]*[{1:0.0000}*{2}^(k−1)+{3}*{4}^(k−1)]", D, M1, lambda1, M2, lambda2);
        }       
    }

    public void PrintLagrangeMatrices()
    {
        Console.WriteLine();
        Console.WriteLine("L1=[ {0:0.0000} {1:0.0000} ]", langrange1[0, 0], langrange1[0, 1]);
        Console.WriteLine("   [ {0:0.0000} {1:0.0000} ]", langrange1[1, 0], langrange1[1, 1]);
        Console.WriteLine();
        Console.WriteLine("L2=[ {0:0.0000} {1:0.0000} ]", langrange2[0, 0], langrange2[0, 1]);
        Console.WriteLine("   [ {0:0.0000} {1:0.0000} ]", langrange2[1, 0], langrange2[1, 1]);
    }
}
