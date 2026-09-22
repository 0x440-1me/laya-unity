# Embedded Laya roadmap

Embedded inference is future research, not part of 0.1.

```text
EmbeddedLayaBackend
        │
        ├── tokenizer
        ├── sequence builder
        ├── ONNX model
        ├── Unity Inference Engine / Sentis
        └── calibration / result post-processing
```

Work must cover ModernBERT tokenizer parity, sequence construction, ONNX graph compatibility, calibration, platform performance, and model distribution. The current FP32 bundle size makes distribution and FP16/INT8 evaluation especially important.

Before embedded support can be declared, deterministic fixtures must compare Python Laya, the receptron Node/ONNX implementation, and Unity within a documented numerical tolerance. The selected label, score behavior, noul probability, tokenization, and probability distributions all need parity coverage.

Model weights must be downloaded separately under their own terms and will never be stored in this Git repository.
