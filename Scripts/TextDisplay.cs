using Godot;
using System;

public partial class TextDisplay : Node2D
{
    [Export]
    private float _flickerUnderscore = 0.5f;
    [Export]
    private float _typingSpeed = 0.05f;

    private RichTextLabel _mainText;
    private bool _isTyping = false;
    private string _currentText = "";

    private float _flickerTimer = 0.0f;
    private float _typingTimer = 0.0f;
    private int _charsDisplayed = 0;

    public override void _Ready()
    {
        base._Ready();

        _mainText = GetNode<RichTextLabel>("Text");
        _mainText.Text = _currentText;
        _charsDisplayed = _currentText.Length;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (_isTyping)
        {
            _typingTimer += (float)delta;
            if (_typingTimer >= _typingSpeed)
            {
                _typingTimer = 0.0f;
                _charsDisplayed++;
                _mainText.Text = _currentText.Substring(0, _charsDisplayed);
                if (_mainText.Text == _currentText)
                {
                    _isTyping = false;
                    _mainText.Text += " ";
                }
                _mainText.ScrollToLine(_mainText.GetLineCount() - 1);
            }
        }
        else
        {
            Flicker(delta);
        }
    }

    private void Flicker(double delta)
    {
        _flickerTimer += (float)delta;
        if (_flickerTimer >= _flickerUnderscore)
        {
            _flickerTimer = 0.0f;
            if (_mainText.Text.EndsWith("_"))
            {
                _mainText.Text = _mainText.Text.TrimEnd('_');
                _mainText.Text += " ";
            }
            else
            {
                _mainText.Text = _mainText.Text.TrimEnd(' ');
                _mainText.Text += "_";
            }
        }
    }

    public void AddLine(string line)
    {
        if (_mainText.Text.EndsWith('_')) _mainText.Text = _mainText.Text.TrimEnd('_');
        else if (_mainText.Text.EndsWith(' ')) _mainText.Text = _mainText.Text.TrimEnd(' ');

        _currentText += line;
        _currentText += "\n";
        _typingTimer = _typingSpeed; // Force immediate first char display

        _flickerTimer = 0.0f;
        _isTyping = true;
    }
}
