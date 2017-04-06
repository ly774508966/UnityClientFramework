//auth: Xiang ChunSong 2015/10/14
//purpose:

using System.Collections;
using System.Text;
using System;
using System.Data;
using System.IO;

//��CSV�ļ���,��ȡָ����CSV�ļ������Ե���DataTable
class CsvStreamReader : DataReader
{
    private ArrayList rowAL;         //������,CSV�ļ���ÿһ�о���һ����

    private Encoding encoding;        //����

    public CsvStreamReader()
    {
        this.rowAL = new ArrayList();
        this.encoding = Encoding.Default;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileName">�ļ���,�����ļ�·��</param>
    public CsvStreamReader(string fileName)
    {
        this.rowAL = new ArrayList();
        filename = fileName;
        this.encoding = Encoding.Default;
        LoadCsvFile();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fileName">�ļ���,�����ļ�·��</param>
    /// <param name="encoding">�ļ�����</param>
    public CsvStreamReader(string fileName, Encoding encoding)
    {
        this.rowAL = new ArrayList();
        filename = fileName;
        this.encoding = encoding;
        LoadCsvFile();
    }

    /// <summary>
    /// �ļ���,�����ļ�·��
    /// </summary>
    protected override void Load(string file)
    {
        LoadCsvFile();
    }

    /// <summary>
    /// �ļ�����
    /// </summary>

    public Encoding FileEncoding
    {
        set
        {
            this.encoding = value;
        }
    }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    protected override int GetRowCount()
    {
        return rowAL.Count;
    }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    protected override int GetColCount()
    {
        int maxCol;

        maxCol = 0;
        for (int i = 0; i < this.rowAL.Count; i++)
        {
            ArrayList colAL = (ArrayList)this.rowAL[i];

            maxCol = (maxCol > colAL.Count) ? maxCol : colAL.Count;
        }

        return maxCol;
    }

    /// <summary>
    /// ��ȡĳ��ĳ�е�����

    /// row:��,row = 1�����һ��

    /// col:��,col = 1�����һ��  
    /// </summary>
    protected override string GetCellContent(int row, int col)
    {
        CheckRowValid(row);
        CheckColValid(col);
        ArrayList colAL = (ArrayList)this.rowAL[row - 1];

        //������������ݴ��ڵ�ǰ�е���ʱ,���ؿ�ֵ

        if (colAL.Count < col)
        {
            return "";
        }

        return colAL[col - 1].ToString();
    }


    /// <summary>
    /// ������С�У�����У���С�У�����У�������һ��DataTable���͵�����

    /// �е���1�����һ��

    /// �е���1�����һ��

    /// maxrow: -1���������
    /// maxcol: -1���������
    /// </summary>
    public DataTable this[int minRow, int maxRow, int minCol, int maxCol]
    {
        get
        {
            //������Ч����֤

            CheckRowValid(minRow);
            CheckMaxRowValid(maxRow);
            CheckColValid(minCol);
            CheckMaxColValid(maxCol);
            if (maxRow == -1)
            {
                maxRow = RowCount;
            }
            if (maxCol == -1)
            {
                maxCol = ColCount;
            }
            if (maxRow < minRow)
            {
                throw new Exception("�����������С����С����");
            }
            if (maxCol < minCol)
            {
                throw new Exception("�����������С����С����");
            }
            DataTable csvDT = new DataTable();
            int i;
            int col;
            int row;

            //������

            for (i = minCol; i <= maxCol; i++)
            {
                csvDT.Columns.Add(i.ToString());
            }
            for (row = minRow; row <= maxRow; row++)
            {
                DataRow csvDR = csvDT.NewRow();

                i = 0;
                for (col = minCol; col <= maxCol; col++)
                {
                    csvDR[i] = this[row, col];
                    i++;
                }
                csvDT.Rows.Add(csvDR);
            }

            return csvDT;
        }
    }


    /// <summary>
    /// ��������Ƿ�����Ч��

    /// </summary>
    /// <param name="col"></param>  
    private void CheckRowValid(int row)
    {
        if (row <= 0)
        {
            throw new Exception("��������С��0");
        }
        if (row > RowCount)
        {
            throw new Exception("û�е�ǰ�е�����" + row);
        }
    }

    /// <summary>
    /// �����������Ƿ�����Ч��

    /// </summary>
    /// <param name="col"></param>  
    private void CheckMaxRowValid(int maxRow)
    {
        if (maxRow <= 0 && maxRow != -1)
        {
            throw new Exception("�������ܵ���0��С��-1");
        }
        if (maxRow > RowCount)
        {
            throw new Exception("û�е�ǰ�е�����");
        }
    }

    /// <summary>
    /// ��������Ƿ�����Ч��

    /// </summary>
    /// <param name="col"></param>  
    private void CheckColValid(int col)
    {
        if (col <= 0)
        {
            throw new Exception("��������С��0");
        }
        if (col > ColCount)
        {
            throw new Exception("û�е�ǰ�е�����");
        }
    }

    /// <summary>
    /// �������������Ƿ�����Ч��

    /// </summary>
    /// <param name="col"></param>  
    private void CheckMaxColValid(int maxCol)
    {
        if (maxCol <= 0 && maxCol != -1)
        {
            throw new Exception("�������ܵ���0��С��-1");
        }
        if (maxCol > ColCount)
        {
            throw new Exception("û�е�ǰ�е�����");
        }
    }

    /// <summary>
    /// ����CSV�ļ�
    /// </summary>
    private void LoadCsvFile()
    {
        //�����ݵ���Ч�Խ�����֤

        if (this.filename == null)
        {
            throw new Exception("��ָ��Ҫ�����CSV�ļ���");
        }
        else if (!File.Exists(this.filename))
        {
            throw new Exception("ָ����CSV�ļ�������");
        }
        else
        {
        }
        if (this.encoding == null)
        {
            this.encoding = Encoding.Default;
        }
        this.rowAL.Clear();

        StreamReader sr = new StreamReader(this.filename, this.encoding);
        string csvDataLine;

        csvDataLine = "";
        while (true)
        {
            string fileDataLine;

            fileDataLine = sr.ReadLine();
            if (fileDataLine == null)
            {
                break;
            }
            if (csvDataLine == "")
            {
                csvDataLine = fileDataLine;//GetDeleteQuotaDataLine(fileDataLine);
            }
            else
            {
                csvDataLine += "\r\n" + fileDataLine;//GetDeleteQuotaDataLine(fileDataLine);
            }
            //�������ż�������ţ�˵�����������г��ֻس������������
            if (!IfOddQuota(csvDataLine))
            {
                AddNewDataLine(csvDataLine);
                csvDataLine = "";
            }
        }
        sr.Close();
        //�����г�������������
        if (csvDataLine.Length > 0)
        {
            throw new Exception("CSV�ļ��ĸ�ʽ�д���");
        }
    }

    /// <summary>
    /// ��ȡ�����������ű�ɵ������ŵ�������
    /// </summary>
    /// <param name="fileDataLine">�ļ�������</param>
    /// <returns></returns>
    private string GetDeleteQuotaDataLine(string fileDataLine)
    {
        return fileDataLine.Replace("\"\"", "\"");
    }

    /// <summary>
    /// �ж��ַ����Ƿ��������������
    /// </summary>
    /// <param name="dataLine">������</param>
    /// <returns>Ϊ����ʱ������Ϊ�棻���򷵻�Ϊ��</returns>
    private bool IfOddQuota(string dataLine)
    {
        int quotaCount;
        bool oddQuota;

        quotaCount = 0;
        for (int i = 0; i < dataLine.Length; i++)
        {
            if (dataLine[i] == '\"')
            {
                quotaCount++;
            }
        }

        oddQuota = false;
        if (quotaCount % 2 == 1)
        {
            oddQuota = true;
        }

        return oddQuota;
    }

    /// <summary>
    /// �ж��Ƿ������������ſ�ʼ

    /// </summary>
    /// <param name="dataCell"></param>
    /// <returns></returns>
    private bool IfOddStartQuota(string dataCell)
    {
        int quotaCount;
        bool oddQuota;

        quotaCount = 0;
        for (int i = 0; i < dataCell.Length; i++)
        {
            if (dataCell[i] == '\"')
            {
                quotaCount++;
            }
            else
            {
                break;
            }
        }

        oddQuota = false;
        if (quotaCount % 2 == 1)
        {
            oddQuota = true;
        }

        return oddQuota;
    }

    /// <summary>
    /// �ж��Ƿ������������Ž�β
    /// </summary>
    /// <param name="dataCell"></param>
    /// <returns></returns>
    private bool IfOddEndQuota(string dataCell)
    {
        int quotaCount;
        bool oddQuota;

        quotaCount = 0;
        for (int i = dataCell.Length - 1; i >= 0; i--)
        {
            if (dataCell[i] == '\"')
            {
                quotaCount++;
            }
            else
            {
                break;
            }
        }

        oddQuota = false;
        if (quotaCount % 2 == 1)
        {
            oddQuota = true;
        }

        return oddQuota;
    }

    /// <summary>
    /// �����µ�������

    /// </summary>
    /// <param name="newDataLine">�µ�������</param>
    private void AddNewDataLine(string newDataLine)
    {
        //System.Diagnostics.Debug.WriteLine("NewLine:" + newDataLine);

        ////return;

        ArrayList colAL = new ArrayList();
        string[] dataArray = newDataLine.Split(',');
        bool oddStartQuota;        //�Ƿ������������ſ�ʼ

        string cellData;

        oddStartQuota = false;
        cellData = "";
        for (int i = 0; i < dataArray.Length; i++)
        {
            if (oddStartQuota)
            {
                //��Ϊǰ���ö��ŷָ�,����Ҫ���϶���
                cellData += "," + dataArray[i];
                //�Ƿ������������Ž�β
                if (IfOddEndQuota(dataArray[i]))
                {
                    colAL.Add(GetHandleData(cellData));
                    oddStartQuota = false;
                    continue;
                }
            }
            else
            {
                //�Ƿ������������ſ�ʼ

                if (IfOddStartQuota(dataArray[i]))
                {
                    //�Ƿ������������Ž�β,������һ��˫����,���Ҳ�������������

                    if (IfOddEndQuota(dataArray[i]) && dataArray[i].Length > 2 && !IfOddQuota(dataArray[i]))
                    {
                        colAL.Add(GetHandleData(dataArray[i]));
                        oddStartQuota = false;
                        continue;
                    }
                    else
                    {

                        oddStartQuota = true;
                        cellData = dataArray[i];
                        continue;
                    }
                }
                else
                {
                    colAL.Add(GetHandleData(dataArray[i]));
                }
            }
        }
        if (oddStartQuota)
        {
            throw new Exception("���ݸ�ʽ������");
        }
        this.rowAL.Add(colAL);
    }


    /// <summary>
    /// ȥ�����ӵ���β���ţ���˫���ű�ɵ�����

    /// </summary>
    /// <param name="fileCellData"></param>
    /// <returns></returns>
    private string GetHandleData(string fileCellData)
    {
        if (fileCellData == "")
        {
            return "";
        }
        if (IfOddStartQuota(fileCellData))
        {
            if (IfOddEndQuota(fileCellData))
            {
                return fileCellData.Substring(1, fileCellData.Length - 2).Replace("\"\"", "\""); //ȥ����β���ţ�Ȼ���˫���ű�ɵ�����
            }
            else
            {
                throw new Exception("���������޷�ƥ��" + fileCellData);
            }
        }
        else
        {
            //��������""    """"      """"""   
            if (fileCellData.Length > 2 && fileCellData[0] == '\"')
            {
                fileCellData = fileCellData.Substring(1, fileCellData.Length - 2).Replace("\"\"", "\""); //ȥ����β���ţ�Ȼ���˫���ű�ɵ�����
            }
        }

        return fileCellData;
    }
}
