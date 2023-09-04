using System;
using System.Collections.Generic;
using UnityEngine;

public class SudokuCell
{
    public SudokuGrid sudokuGrid;
    public Vector2Int gridPosition;
    public byte? number;
    public byte expectedNumber;
    public List<byte> candidateNumbers;

    public Action<SudokuCell> onRemoveNumber;
    public Action<SudokuCell, byte> onSetNumber;
    public Action<SudokuCell, byte> onAddCandidateNumber;
    public Action<SudokuCell, byte> onRemoveCandidateNumber;

    public SudokuCell CreateCopy()
    {
        SudokuCell cell = new SudokuCell();

        cell.sudokuGrid = null;
        cell.gridPosition = gridPosition;
        cell.number = number;
        cell.expectedNumber = expectedNumber;
        cell.candidateNumbers = new List<byte>(candidateNumbers);

        return cell;
    }

    public void ClearCandidateNumbers()
    {
        candidateNumbers = new List<byte>();
    }

    public void SetAllCandidateNumbers()
    {
        candidateNumbers = new List<byte>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    }

    public void Clear()
    {
        number = null;

        SetAllCandidateNumbers();
    }

    public void RemoveNumber()
    {
        number = null;

        // How manage candidate numbers here ??

        onRemoveNumber?.Invoke(this);
    }

    public void SetNumber(byte cellNumber)
    { 
        number = cellNumber;

        // meybe cell RemoveCandidateNumberFromAllIntersectingCells here

        onSetNumber?.Invoke(this, cellNumber);
    }

    public void AddToCandidateNumbers(byte numberToAdd)
    {
        candidateNumbers.Add(numberToAdd);

        onAddCandidateNumber?.Invoke(this, numberToAdd);
    }

    public void RemoveFromCandidateNumbers(byte numberToRemove)
    {
        candidateNumbers.Remove(numberToRemove);

        onRemoveCandidateNumber?.Invoke(this, numberToRemove);
    }

    public bool HaveCandidateNumbers()
    {
        return candidateNumbers.Count > 0;
    }

    public bool HaveOnlyOneCandidateNumber()
    {
        return candidateNumbers.Count == 1;
    }

    public byte GetRandomCandidateNumber()
    {
        return candidateNumbers[UnityEngine.Random.Range(0, candidateNumbers.Count)];
    }

    /*public bool SetNumberIfOnlyOneCandidateNumber()
    {
        if (HaveOnlyOneCandidateNumber() == false)
            return false;

        number = candidateNumbers[0];

        return true;
    }*/
}